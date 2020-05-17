local jb = require("JobLib.lua")
local demo = function()
   -- function声明
   local f1 = function(item)
       log("向南1米")
       CallMgr:AddInvoke(function() item:Success() end, 1)
   end

   local f2 = function(item)
       log("向东1米")
       CallMgr:AddInvoke(function() item:Success() end, 1)
   end

   local f3 = function(item)
       log("向北1米")
      CallMgr:AddInvoke(function() item:Success() end, 1)
   end

   local f4 = function(item)
       log("向西1米")
       CallMgr:AddInvoke(function() item:Success() end, 1)
   end

   local f5 = function(item)
       log("向上1米")
       CallMgr:AddInvoke(function() item:Success() end, 1)
   end

   local successFind = function(item)
       CallMgr:AddInvoke(function() log("发现宝藏.") item:Success() end, 1)
   end

   local successFind2 = function(item)
       local f = function()
        log("发现宝藏.")
       end
       item._OnReset = function(i)
            f = function() end
       end
       CallMgr:AddInvoke(function() f() item:Success() end, 2)
   end

   local failFind = function(item)
       CallMgr:AddInvoke(function() log("没有宝藏.") item:Fail() end, 1)
   end

   local logFunc = function (item)
        log("------------------------------------\n\n")
        item:Success()
   end


   -- -- 实现方式：通过配置直接生成，写法最简洁易懂
   -- local d1 = {
   --  {f1, f2, type = jb.Type.Paral},  -- type为nil是串行，Lib.Type.Paral是并行
   --  f3,
   --  f4,
   --  f5
   -- }
   -- print("\n\n")
   -- local root3 = jb.BuildBuyFuncDict(d1)
   -- root3:Run()

   -- 下面是复杂逻辑：
   -- 从A、B两个路线依次寻找宝藏：A.先往南走1米，然后往东1米，寻找  B.找不到则往北1米，往西1米，再找  3.找不到宝藏就此结束，找到则再向上走1米

   -- 情景1：同时进行AB两条路线寻找，当有一个找到时不再寻找
   local d4 =
   {
    {
      type = jb.Type.ParalSelector,
      {f1, f2, successFind},
      {f3, f4, successFind2},
    },
    f5,
   }
   local root6 = jb.BuildBuyFuncDict(d4)

   -- 情景2：B路线找到了宝藏
   local d2 =
   {
    {
      type = jb.Type.SeqSelector,
      {f1, f2, failFind},
      {f3, f4, successFind},
      {f3, f4, successFind},
    },
    f5,
   }
   local root4 = jb.BuildBuyFuncDict(d2)

   -- 情景3：AB路线都没找到
   local d3 =
   {
    {
      type = jb.Type.SeqSelector,
      {f1, f2, failFind},
      {f3, f4, failFind},
    },
    f5,
   }

   local root5 = jb.BuildBuyFuncDict(d3)

   local seq = jb.Seq.New()
   seq:AddChild(root6)
   seq:AddAction(logFunc)
   seq:AddChild(root4)
   seq:AddAction(logFunc)
   seq:AddChild(root5)
   seq:Run(function()
      log("end1")
    end, function()
      log("end2")
    end)
end

demo()