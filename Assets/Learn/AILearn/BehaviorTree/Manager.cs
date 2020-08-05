using System;
using System.Collections.Generic;

namespace AILearn
{
    namespace BehaviorTree
    {
        static class Manager
        {
            static readonly Dictionary<int, NodeBase> BTRootNodes = new Dictionary<int, NodeBase>();
            static readonly Dictionary<int, NodeBase> BTNormalNodes = new Dictionary<int, NodeBase>();
            static int BTCurrentIndex = 1;

            // Tick一颗行为树
            static void TickOne(int rootId)
            {
                NodeBase rootNodeBase = BTRootNodes[rootId];
                Global.IsRecursionOk = true;
                if (rootNodeBase.Evaluate() && Global.IsRecursionOk)
                {
                    rootNodeBase.Tick();
                }
            }

            // Tick全部行为树
            static void TickAll()
            {
                foreach (var it in BTRootNodes)
                {
                    Global.IsRecursionOk = true;
                    if (it.Value.Evaluate() && Global.IsRecursionOk)
                    {
                        it.Value.Tick();
                    }
                }
            }

            // 创建一个RootNode
            static int CreateRootNode()
            {
                NodeBase root = new Selector(null);
                BTRootNodes[BTCurrentIndex] = root;
                BTNormalNodes[BTCurrentIndex] = root;
                return BTCurrentIndex++;
            }

            // 为Node设置PreCondition
            static void SetNodePreCondition(int nodeId, Func<bool> dyOnJudge)
            {
                NodeBase nodeBase = BTNormalNodes[nodeId];
                nodeBase.SetPreCondition(dyOnJudge);
            }

            // 创建一个action节点
            static int CreateAction(int parentId, System.Action dyOnEnter, Func<TickStatus> dyOnExecute, Action<TickStatus> dyOnExit)
            {
                NodeBase parent = BTNormalNodes[parentId];
                Action nodeBase = new Action(parent);
                parent.AddChild(nodeBase);
                nodeBase.SetDyOnEnter(dyOnEnter);
                nodeBase.SetDyOnExit(dyOnExit);
                nodeBase.SetDyOnExecute(dyOnExecute);
                BTNormalNodes[BTCurrentIndex] = nodeBase;
                return BTCurrentIndex++;
            }

            // 创建一个串行节点
            static int CreateSequence(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                var node = new Sequence(parent);
                parent.AddChild(node);
                BTNormalNodes[BTCurrentIndex] = node;
                return BTCurrentIndex++;
            }

            // 创建一个循环节点
            static int CreateLoopNode(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                BTNormalNodes[BTCurrentIndex] = new Sequence(parent);
                return BTCurrentIndex++;
            }

            // 创建一个并行节点
            static int CreateParallel(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                var node = new Parallel(parent);
                parent.AddChild(node);
                BTNormalNodes[BTCurrentIndex] = node;
                return BTCurrentIndex++;
            }

            // 创建一个普通选择节点
            static int CreateSelector(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                var node = new Selector(parent);
                parent.AddChild(node);
                BTNormalNodes[BTCurrentIndex] = node;
                return BTCurrentIndex++;
            }

            // 创建一个非递归选择节点
            static int CreateSelectorNoRecursion(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                var node = new SelectorNoRecursion(parent);
                parent.AddChild(node);
                BTNormalNodes[BTCurrentIndex] = node;
                return BTCurrentIndex++;
            }

            // 创建一个优先选择节点
            static int CreateSelectorPriority(int parentId)
            {
                NodeBase parent = BTNormalNodes[parentId];
                var node = new SelectorPriority(parent);
                parent.AddChild(node);
                BTNormalNodes[BTCurrentIndex] = node;
                return BTCurrentIndex++;
            }

            static void Init()
            {
                BTCurrentIndex = 1;
            }

            static void Destory()
            {
                BTRootNodes.Clear();
                BTNormalNodes.Clear();
            }

            static void DestoryOne(int rootId)
            {
                BTRootNodes.Remove(rootId);
            }
        }
    }
}