---
--- 类似学分系统升级时，需要按照一定时序弹UI飘字，封装一下避免死代码
--- Job是一个类似于Pipeline的工具类，节点可以动态添加，节点可以复用
--- 也类似于Cocos中的action
--- 也类似于DoTween中的Sequence动画与Parallel动画
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
----------------------------------------------串行--------------------------------------------------------------

local SequenceJob = class("SequenceJob", Job)

function SequenceJob:_OnInit()
    self.m_Children = {}
    self.m_CurIdx = 1
    self.m_ChildSuccessHandler = function(job)
        self:ChildSuccessHandler(job)
    end
    self.m_ChildFailHandler = function(job)
        self:ChildFailHandler(job)
    end
    self.m_ChildProgressChangeHandler = function(job)
        self:ChildProgressChangeHandler(job)
    end
end

-- 参数类型 Action<Job>
function SequenceJob:AddAction(action)
    local job = Job.New(action)
    self:AddChild(job)
    return job
end

function SequenceJob:AddChild(job)
    --if self.m_IsRunning then
        --Debug.LogError("SequenceJob 已经启动，无法添加Child.")
    --else
        table.insert(self.m_Children, job)
    --end
end

function SequenceJob:ChildSuccessHandler(child)
      self.m_CurIdx = self.m_CurIdx + 1
      self:NextJob()
end

function SequenceJob:ChildFailHandler(child)
    self:Fail()
end

function SequenceJob:ChildProgressChangeHandler(child)
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
    if not self.m_ProgressChangeCall then
        -- 如果不需要知道进度，就不再计算
        self.m_ChildProgressChangeHandler = nil
    end
    self.m_CurIdx = 1
    self:NextJob()
end

function SequenceJob:_OnReset()
    for _,v in ipairs(self.m_Children) do
        v:Reset()
    end
     self.m_CurIdx = 1
end

function SequenceJob:_OnDestroy()
    for _,v in ipairs(self.m_Children) do
        v:Destroy()
    end
end

----------------------------------------------并行--------------------------------------------------------------
local ParallelJob = class("ParallelJob", Job)

function ParallelJob:_OnInit()
    self.m_Children = {}
    self.m_SuccessCount = 0
    self.m_childIndexToProgress = {}  -- 已经结束的索引
    self.m_ErrorCount = 0
    self.m_ErrorChildren = {}
    self.m_ChildSuccessHandler = function(job)
        self:ChildSuccessHandler(job)
    end
    self.m_ChildFailHandler = function(job)
        self:ChildFailHandler(job)
    end
    self.m_ChildProgressChangeHandler = function(job)
        self:ChildProgressChangeHandler(job)
    end
end

-- 参数类型 Action<Job>
function ParallelJob:AddAction(action)
    local job = Job.New(action)
    self:AddChild(job)
    return job
end

 function ParallelJob:AddChild(job)
     if self.m_IsRunning then
         --log.error("ParallelJob 已经启动，无法添加Child.")
         table.insert(self.m_Children, job)
         job:Run(self.m_ChildSuccessHandler, self.m_ChildFailHandler, self.m_ChildProgressChangeHandler, #self.m_Children)
     else
         table.insert(self.m_Children, job)
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
       for _, v in ipairs(self.m_Children) do
           v:Reset()
       end
       self.m_SuccessCount = 0
       self.m_ErrorCount = 0
       self.m_ErrorChildren = {}
       self.m_childIndexToProgress = {}
    end

    function ParallelJob:_OnDestroy()
        for _,v in ipairs(self.m_Children) do
            v:Destroy()
        end
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

   function ParallelJob:ChildSuccessHandler(child)
       if not self.m_IsRunning then
           return
       end
       self.m_SuccessCount = self.m_SuccessCount + 1
       self:TryEnd()
    end

   function ParallelJob:ChildFailHandler(child)
       if not self.m_IsRunning then
           return
        end
       self.m_ErrorCount = self.m_ErrorCount + 1
       table.insert( self.m_ErrorChildren, child)
       self:TryEnd()
    end

   function ParallelJob:ChildProgressChangeHandler(child)
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

----------------------------------生产者消费者：瞬间生产, seq方式依次消费----------------------------------
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

local ProducerConsumerSeq = class("ProducerConsumerSeq", Job)

function ProducerConsumerSeq:_OnInit()
    self.seqJob = nil
    self._queue = Queue.New()
end

-- 外部接口： 生产
function ProducerConsumerSeq:Product(data)
    self._queue:Enqueue(data)
end

-- 外部接口：消费
-- 参数类型：Action<Job, data>， data是本次消费的数据，job是本次消费的job，调用job:Success() 表示一次消费结束
function ProducerConsumerSeq:Consume(context)
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
---------------------------------------------------------------------------------------------------------
local JobLib = {
    -- 类在这里，方便继承扩展
    JobClass = Job,
    SequenceJobClass = SequenceJob,
    ParallelJobClass = ParallelJob,
    ProducerConsumerSeqClass = ProducerConsumerSeq,
}

-- 新建一个动作节点
-- 执行的程序都在context中，类型是Action<Job>
-- 异步或同步调用结束之后，执行item:Success()表示此job成功，执行item:Fail()表示此job失败
function JobLib.Job(context)
    return Job.New(context)
end

-- 新建一个串行节点，子节点顺序执行，遇到错误则直接FailCallback，全部正确SuccessCallback
function JobLib.Seq()
    return SequenceJob.New()
end

-- 新建一个并行节点，子节点并行执行，全部执行完毕，有错则FailCallback，全部正确SuccessCallback
function JobLib.Paral()
    return ParallelJob.New()
end

-- 新建一个串行节点，并添加到parent上
function JobLib.AddSeqChild(parent)
    local child = SequenceJob.New()
    parent:AddChild(child)
    return child
end

-- 新建一个并行节点并添加到parent上
function JobLib.AddParalChild(parent)
    local child = ParallelJob.New()
    parent:AddChild(child)
    return child
end

-- 获取一个生产者消费者对象
function JobLib.ProducerConsumerSeq()
    return ProducerConsumerSeq.New()
end

return JobLib