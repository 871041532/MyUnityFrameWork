
--[[
任务树
介绍：
  按一定时序执行异步任务。应对策划的需求更改，方便快速拆分组合。(如有需求可以支持可视化节点编辑。)

与行为树和决策树的区别：
  行为树和决策树更偏向于AI的思考决策，倾向于顺时决策，靠Tick驱动。任务树偏向于行为，倾向于执行一系列时序逻辑，不靠Tick驱动，而是某个cell的成功succellCall或失败failCall驱动，并且可以实时获取当前执行进度。

Job:
  异步执行的动作基类。在异步的context执行过程中，self:success()表示执行成功，self:fail()表示执行失败。可以直接使用或继承扩展。

SequenceJob：
  串行复合类。从first开始，序列执行子节点。当前子节点执行成功，自动开始执行下一个子节点；当前子节点执行失败sequence终止且失败；所有子节点都成功则sequence执行成功。

SeqSelectorJob: 
  串行选择复合类。从first开始，序列执行子节点。当前子节点执行成功，seqSelector终止且成功；所有子节点都失败则seqSelector执行失败。

ParallerJob:
  并行复合类。同时执行所有子节点，等待所有子节点执行完毕。如果子节点全部执行成功，则parallel执行成功；如果有子节点执行失败，则parallel执行失败。

ParalSelectorJob:
  并行选择复合类。同时执行所有子节点，等待第一个执行成功的子节点。当第一个子节点执行成功时paralSelector完成且成功。如果子节点全部执行失败，则paralSelector执行失败。

demo: 将文件底部的注释放开，可单独运行此文件。
--]]

-- 为了可以单独执行此lua文件搞个local class
local class = function(name, super)
    if super ~= nil then
        if type(super) ~= "table" then
            return
        end
    end
    local class_type = nil
    if super then
        class_type = {}
        setmetatable(class_type, {__index = super})
        class_type.__super = super
        class_type.super = super
    else
        class_type = {}
    end
    class_type.__index = class_type
    class_type.__cname = name
    class_type.__ctype = 2

    class_type.New = function(...)
        local instance = setmetatable({}, class_type)
        instance.class = class_type
        instance:ctor(...)
        return instance
    end
    class_type.new = class_type.New
    return class_type
end

------------- 基类-----------------
-- 执行的程序都在context中，类型是Action<Job>
-- 异步或同步调用结束之后，执行item:Success()表示此job成功，执行item:Fail()表示此job失败
local Job = class("Job")

function Job:ctor(context)  -- runProcess可空
    self.m_SuccessCall = nil
    self.m_ErrorCall = nil
    self.m_ProgressChangeCall = nil
    self.m_Id = -1
    self.m_Progress = -1
    self.m_IsRunning = false

    self.m_OnRun = context  -- Action<Job>
    self.m_IsDestroyed = false
    self:_OnInit()
end

-- Action<Job>
function Job:Run(successCall, errorCall, processChangeCall,   _runId)
    if self.m_IsDestroyed then
        log.error("Job is destroyed, do not run!")
        return
    end
    if self.m_IsRunning then
        log.error("Job is on running, do not repeat!")
        return
    end
    self.m_SuccessCall = successCall
    self.m_ErrorCall = errorCall
    self.m_ProgressChangeCall = processChangeCall
    self.m_Id = _runId
    self.m_IsRunning = true
    self:ProgressChange(0)
    if self.m_OnRun == nil then
        self:_OnRun()
    else
        self.m_OnRun(self)
    end
end

function Job:Success()
    if self.m_IsDestroyed then
        return
    end
    if not self.m_IsRunning then
        return
    end
    self:ProgressChange(1)
    local successCall = self.m_SuccessCall
    self:Reset()  -- 此处使用尾递归，优化堆栈
    if successCall then
        successCall(self)
    end
end

function Job:Fail()
    if self.m_IsDestroyed then
        return
    end
    if not self.m_IsRunning then
        return
    end
    local m_ErrorCall = self.m_ErrorCall
    self:Reset()  -- 此处使用尾递归，优化堆栈
    if m_ErrorCall then
        m_ErrorCall(self)
    end
end

function Job:ProgressChange(progress)
    if self.m_IsDestroyed then
        return
    end
    if math.abs(progress - self.m_Progress) >= 0.001 then
        self.m_Progress = progress
        if self.m_ProgressChangeCall then
            self.m_ProgressChangeCall(self)
        end
    end
end

function Job:Reset()
    if not self.m_IsRunning then
        return
    end
    self.m_IsRunning = false
    self.m_SuccessCall = nil
    self.m_ProgressChangeCall = nil
    self.m_ErrorCall = nil
    self.m_Id = -1
    self.m_Progress = -1
    self:_OnReset()
end

function Job:Destroy()
    self.m_IsDestroyed = true
    self:_OnDestroy()
end

-- virtual
function Job:_OnInit()
end

-- virtual
function Job:_OnRun()
end

-- virtual
function Job:_OnReset()
end

-- virtual
function Job:_OnDestroy()
end


------------------------ 复合基类 ---------------------
local ComplexJob = class("ComplexJob", Job)

function ComplexJob:ctor(...)
  ComplexJob.super.ctor(self, ...)
  self.m_Children = {}
  self.m_ChildSuccessHandler = function(job)
      self:_OnChildSuccess(job)
  end
  self.m_ChildFailHandler = function(job)
      self:_OnChildFail(job)
  end
  self.m_ChildProgressChangeHandler = function(job)
      self:_OnChildProgressChange(job)
   end
end

function ComplexJob:AddAction(action)
  local job = Job.New(action)
  self:AddChild(job)
  return job
end

function ComplexJob:AddChild(job)
  table.insert(self.m_Children, job)
end


function ComplexJob:_OnChildSuccess(child)
end

function ComplexJob:_OnChildFail(child)
end

function ComplexJob:_OnChildProgressChange(child)
end

function ComplexJob:Reset()
    ComplexJob.super.Reset(self)
    for _,v in ipairs(self.m_Children) do
        v:Reset()
    end
end

function ComplexJob:Destroy()
    ComplexJob.super.Destroy(self)
    for _,v in ipairs(self.m_Children) do
        v:Destroy()
    end
end

------------------------串行----------------------------------
local SequenceJob = class("SequenceJob", ComplexJob)

function SequenceJob:_OnInit()
    self.m_CurIdx = 1
end

function SequenceJob:AddChild(job)
    table.insert(self.m_Children, job)
end

function SequenceJob:_OnChildSuccess(child)
    self.m_CurIdx = self.m_CurIdx + 1
    self:NextJob()
end

function SequenceJob:_OnChildFail(child)
    self:Fail()
end

function SequenceJob:_OnChildProgressChange(child)
    local progress = (self.m_CurIdx -1 + (child.m_Progress >= 0 and child.m_Progress or 0)) / #self.m_Children
    self:ProgressChange(progress)
end

function SequenceJob:NextJob()
    if not self.m_IsRunning then
        return
    end

    if #self.m_Children >= self.m_CurIdx then
        self.m_Children[self.m_CurIdx]:Run(self.m_ChildSuccessHandler, self.m_ChildFailHandler, self.m_ChildProgressChangeHandler, self.m_CurIdx)
    else
        self:Success()
    end
end

function SequenceJob:_OnRun()
  if #self.m_Children == 0 then
    self:Success()
  else
    if not self.m_ProgressChangeCall then
      -- 如果不需要知道进度，就不再计算
      self.m_ChildProgressChangeHandler = nil
    end
    self.m_CurIdx = 1
    self:NextJob()
  end
end

function SequenceJob:_OnReset()
    self.m_CurIdx = 1
end

-------------------------- 串行选择 -----------------------------
local SeqSelectorJob = class("SeqSelectorJob", SequenceJob)

function SeqSelectorJob:_OnChildSuccess(child)
  self:Success()
end

function SeqSelectorJob:_OnChildFail(child)
  self.m_CurIdx = self.m_CurIdx + 1
  self:NextJob()
end

function SeqSelectorJob:_OnChildProgressChange(child)
  self:ProgressChange(child.m_Progress)
end

function SeqSelectorJob:NextJob()
    if not self.m_IsRunning then
        return
    end
    if #self.m_Children >= self.m_CurIdx then
        self.m_Children[self.m_CurIdx]:Run(self.m_ChildSuccessHandler, self.m_ChildFailHandler, self.m_ChildProgressChangeHandler, self.m_CurIdx)
    else
        self:Fail()
    end
end

----------------------并行-------------------------
local ParallelJob = class("ParallelJob", ComplexJob)

function ParallelJob:_OnInit()
  self.m_SuccessCount = 0
  self.m_childIndexToProgress = {}  -- 已经结束的索引
  self.m_ErrorCount = 0
  self.m_ErrorChildren = {}
end

function ParallelJob:AddChild(job)
  ParallelJob.super.AddChild(self, job)
  if self.m_IsRunning then
    job:Run(self.m_ChildSuccessHandler, self.m_ChildFailHandler, self.m_ChildProgressChangeHandler, #self.m_Children)
  end
end

function ParallelJob:_OnRun()
  if not self.m_ProgressChangeCall then
    -- 如果不需要知道进度，就不再计算
    self.m_ChildProgressChangeHandler = nil
  end
  if #self.m_Children <= 0 then
    self:TryEnd()
  else
    for i = 1, #self.m_Children do
      self.m_Children[i]:Run(self.m_ChildSuccessHandler, self.m_ChildFailHandler, self.m_ChildProgressChangeHandler, i)
    end
  end
end

function ParallelJob:_OnReset()
  self.m_SuccessCount = 0
  self.m_ErrorCount = 0
  self.m_ErrorChildren = {}
  self.m_childIndexToProgress = {}
end

function ParallelJob:TryEnd()
  -- 等待所有子Job执行完再出结果
  if (self.m_SuccessCount + self.m_ErrorCount == #self.m_Children) then
    if self.m_ErrorCount > 0 then
      self:Fail()
    else
      self:Success()
    end
  end
end

function ParallelJob:_OnChildSuccess(child)
  if not self.m_IsRunning then
    return
  end
  self.m_SuccessCount = self.m_SuccessCount + 1
  self:TryEnd()
end

function ParallelJob:_OnChildFail(child)
  if not self.m_IsRunning then
    return
  end
  self.m_ErrorCount = self.m_ErrorCount + 1
  table.insert( self.m_ErrorChildren, child)
  self:TryEnd()
end

function ParallelJob:_OnChildProgressChange(child)
  if not self.m_IsRunning then
    return
  end
  self.m_childIndexToProgress[child.m_Id] = child.m_Progress
  local childrenProgress = 0
  for i, v in pairs(self.m_Children) do
    if self.m_childIndexToProgress[i] then
      childrenProgress = childrenProgress + self.m_childIndexToProgress[i]
    else
      childrenProgress = childrenProgress + (v.m_Progress >= 0 and v.m_Progress or 0)
    end
  end
  childrenProgress = childrenProgress / #self.m_Children
  self:ProgressChange(childrenProgress)
end

------------- 并行选择------------------
local ParalSelectorJob = class("ParalSelectorJob", ParallelJob)

function ParalSelectorJob:TryEnd()
  if #self.m_Children == 0 then
    self:Success()
  else
    if self.m_SuccessCount > 0 then
        self:Success()
    elseif self.m_ErrorCount == #self.m_Children then
       self:Fail()
    end
  end
end

function ParalSelectorJob:_OnChildProgressChange(child)
  if not self.m_IsRunning then
    return
  end
  if not self.__childMaxProgress then
    self.__childMaxProgress = 0
  end
  self.__childMaxProgress = math.max(self.__childMaxProgress, child.m_Progress);
  self:ProgressChange(self.__childMaxProgress)
end

-------------生产者消费者：瞬间生产, seq方式依次消费-----------------
local Queue = class("Queue")

function Queue:ctor()
	self._data = {}
	self._tailIndex = 1
	self._headIndex = 1
end

function Queue:IsEmpty()
	return self._headIndex == self._tailIndex
end

function Queue:NotEmpty()
	return self._headIndex ~= self._tailIndex
end

function Queue:Enqueue(item)
	self._headIndex = self._headIndex + 1
	self._data[self._headIndex] = item
end

function Queue:Dequeue()
	if self._headIndex == self._tailIndex then
		return nil
	else
		self._tailIndex = self._tailIndex + 1
		return self._data[self._tailIndex]
	end
end

local PCSeq = class("PCSeq", Job)

function PCSeq:_OnInit()
  self.seqJob = nil
  self._queue = Queue.New()
end

-- 外部接口： 生产
function PCSeq:Product(data)
  self._queue:Enqueue(data)
end

-- 外部接口：消费
-- 参数类型：Action<Job, data>， data是本次消费的数据，job是本次消费的job，调用job:Success() 表示一次消费结束
function PCSeq:Consume(context)
  -- 数据中没东西直接不处理
  if self._queue:IsEmpty() then
    return
  end

  local consumer = function(item)
    local data = self._queue:Dequeue()
    if data then
      context(item, data)
    else
      item:Fail()
    end
  end

  if not self.seqJob then
    -- 没有消费者在消费中
    self.seqJob = SequenceJob.New()
    self.seqJob:AddAction(consumer)
    self.seqJob:Run(function(item2)
      self.seqJob:Destroy()
      self.seqJob = nil
    end, function(item2)
      self.seqJob:Destroy()
      self.seqJob = nil
    end)
  else
    -- 已有消费者在消费中
    self.seqJob:AddAction(consumer)
  end
end

----------------------------------------------------------------------
local Lib = {
  Job = Job,
  Seq = SequenceJob,
  Paral = ParallelJob,
  SeqSelector = SeqSelectorJob,
  ParalSelector = ParalSelectorJob,
  PCSeq = PCSeq,

  Type = {
    Seq = nil,
    Paral = 1,
    SeqSelector = 2,
    ParalSelector = 3,
  },
}

-- 新建一个串行节点，并添加到parent上
function Lib.AddSeqChild(parent)
    local child = SequenceJob.New()
    parent:AddChild(child)
    return child
end

-- 新建一个并行节点并添加到parent上
function Lib.AddParalChild(parent)
    local child = ParallelJob.New()
    parent:AddChild(child)
    return child
end

-- 新建一个选择节点添加到parent上
function Lib.AddSelectorChild(parent)
    local child = SeqSelectorJob.New()
    parent:AddChild(child)
    return child
end

-- 获取一个生产者消费者对象
function Lib.PCSeq()
    return PCSeq.New()
end

-- 直接用funcDictbuild出一个组合job
local _buildFunc
local _createJobBuyType
function Lib.BuildBuyFuncDict(curData)
    if (not curData) or (not next(curData)) then
        return
    end
    local root = _createJobBuyType(curData.type)
    if root then
        _buildFunc(curData, root)
        return root
    end
end

_buildFunc = function(curData, root)
    local dataType = type(curData)
    if dataType == "function" then
        root:AddAction(curData)
    elseif dataType == "table" then
        local curJob = _createJobBuyType(curData.type)
        root:AddChild(curJob)
        for _,v in ipairs(curData) do
            _buildFunc(v, curJob)
        end
    end
end

_createJobBuyType = function(type)
    local curJob
    if type == Lib.Type.Seq then
        curJob = SequenceJob.New()
    elseif type == Lib.Type.Paral then
        curJob = ParallelJob.New()
    elseif type == Lib.Type.SeqSelector then
        curJob = SeqSelectorJob.New()
    elseif type == Lib.Type.ParalSelector then
        curJob = ParalSelectorJob.New()
    end
    return curJob
end


-- local demo = function()
--    -- function声明
--    local f1 = function(item)
--        print("向南1米")
--        item:Success()
--    end

--    local f2 = function(item)
--        print("向东1米")
--        item:Success()
--    end

--    local f3 = function(item)
--        print("向北1米")
--        item:Success()
--    end

--    local f4 = function(item)
--        print("向西1米")
--        item:Success()
--    end

--    local f5 = function(item)
--        print("向上1米")
--        item:Success()
--    end

--    local successFind = function(item)
--        print("发现宝藏.")
--        item:Success()
--    end

--    local failFind = function(item)
--        print("没有宝藏.")
--        item:Fail()
--    end

--    -- 需求：
--    -- s1.并行执行任务1、2    s2.前面都ok后串行执行后续任务
--    -- 翻译一下：1.同时向东又向南一米 2.然后向北1米 3.然后向西1米 4.然后向上1米

--    -- 实现方式1: 先创建所有job，再组合，写法比较繁琐
--    local j1 = Lib.Job.New(f1)
--    local j2 = Lib.Job.New(f2)
--    local j3 = Lib.Job.New(f3)
--    local j4 = Lib.Job.New(f4)
--    local j5 = Lib.Job.New(f5)
--    local root1 = Lib.Seq.New()
--    local p1 = Lib.Paral.New()
--    p1:AddChild(j1)
--    p1:AddChild(j2)
--    root1:AddChild(p1)
--    root1:AddChild(j3)
--    root1:AddChild(j4)
--    root1:AddChild(j5)
--    print("\n\n")
--    root1:Run()

--    -- 实现方式2：边创建边组合，写法简洁一些
--    local root2 = Lib.Seq.New()
--    local p2 = Lib.AddParalChild(root2)
--    p2:AddAction(f1)
--    p2:AddAction(f2)
--    root2:AddAction(f3)
--    root2:AddAction(f4)
--    root2:AddAction(f5)
--    print("\n\n")
--    root2:Run()

--    -- 实现方式3：通过配置直接生成，写法最简洁易懂
--    local d1 = {
--     {f1, f2, type = Lib.Type.Paral},  -- type为nil或不写是串行
--     f3,
--     f4,
--     f5
--    }
--    print("\n\n")
--    local root3 = Lib.BuildBuyFuncDict(d1)
--    root3:Run()

--    -- 下面是复杂逻辑：
--    -- 从A、B两个路线依次寻找宝藏：A.先往南走1米，然后往东1米，寻找  B.找不到则往北1米，往西1米，再找  3.找不到宝藏就此结束，找到则再向上走1米
--    -- 情景1：B路线找到了宝藏
--    local d2 =
--    {
--     {
--       type = Lib.Type.SeqSelector,
--       {f1, f2, failFind},
--       {f3, f4, successFind},
--       {f3, f4, successFind},
--     },
--     f5,
--    }
--    local root4 = Lib.BuildBuyFuncDict(d2)
--    print("\n\n")
--    root4:Run(function()
--        print("最终成功")
--    end, function()
--        print("最终失败")
--    end)

--    -- 情景2：AB路线都没找到
--    local d3 =
--    {
--     {
--       type = Lib.Type.SeqSelector,
--       {f1, f2, failFind},
--       {f3, f4, failFind},
--     },
--     f5,
--    }
--    local root5 = Lib.BuildBuyFuncDict(d3)
--    print("\n\n")
--    root5:Run(function()
--        print("最终成功")
--    end, function()
--        print("最终失败")
--    end)
-- end

-- demo()

return Lib