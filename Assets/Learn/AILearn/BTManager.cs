

using System;
using System.Collections.Generic;

namespace AILearn
{
	
class BTManager
{
	Dictionary<int, BTNode> BTRootNodes;
	Dictionary<int, BTNode> BTNormalNodes;
	int BTCurrentIndex = 1;

	void TickOne(int rootId)
	{
		BTNode rootNode = BTRootNodes[rootId];
		BTGlobal.RECURSION_OK = true;
		if (rootNode.Evaluate() && BTGlobal.RECURSION_OK)
		{
			rootNode.Tick();
		}
	}

 void TickAll()
{
	foreach (var it in BTRootNodes)
	{
		BTGlobal.RECURSION_OK = true;
		if (it.Value.Evaluate() && BTGlobal.RECURSION_OK)
		{
			it.Value.Tick();
		}
	}
}
	
 int CreateRootNode()
{
	BTNode root = new BTNodePrioritySelector(null);
	BTRootNodes[BTCurrentIndex] = root;
	BTNormalNodes[BTCurrentIndex] = root;
	return BTCurrentIndex++;
}

	void NodeSetPreCondition(int nodeId, Func<bool> dynamicjudge)
{
	BTNode node = BTNormalNodes[nodeId];
	node.SetPreCondition(dynamicjudge);
}

 int CreateTeminalNode(int parentId, Func<int> dynamicOnExcute)
{
	BTNode parent = BTNormalNodes[parentId];
	BTNodeTerminal* node = new BTNodeTerminal(parent);
	parent->AddChildNode(node);
	node->SetDynamicOnExecute([dynamicOnExcute](const BTNodeInputParam&input, const BTNodeOutputParam&output) ->StatusBTRunning {
		StatusBTRunning result = (StatusBTRunning)dynamicOnExcute();
		return result;
	});
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}
Export uint CreateSequenceNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	auto node = new BTNodeSequence(parent);
	parent->AddChildNode(node);
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}
Export uint CreateLoopNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	BTNormalNodes[BTCurrentIndex] = new BTNodeSequence(parent);
	return BTCurrentIndex++;
}
Export uint CreateParallelNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	auto node = new BTNodeParallel(parent);
	parent->AddChildNode(node);
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}

Export uint CreatePrioritySelectorNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	auto node = new BTNodePrioritySelector(parent);
	parent->AddChildNode(node);
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}

Export uint CreateNoRecursionPrioritySelectorNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	auto node = new BTNodeNoRecursionPrioritySelector(parent);
	parent->AddChildNode(node);
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}

Export uint CreateNonePrioritySelectorNode(uint parentId)
{
	BTNode* parent = BTNormalNodes[parentId];
	auto node = new BTNodeNonePrioritySelector(parent);
	parent->AddChildNode(node);
	BTNormalNodes[BTCurrentIndex] = node;
	return BTCurrentIndex++;
}

Export void BTInit()
{
	BTCurrentIndex = 1;
}

Export void BTDestory()
{
	for (auto&& it : BTRootNodes)
	{
		delete it.second;
	}
	BTRootNodes.clear();
	BTNormalNodes.clear();  // ԭʼ��ƣ��ӽڵ��ɸ��ڵ�������
}

Export void BTDestoryOne(uint rootId)
{
	delete BTRootNodes[rootId];
	BTRootNodes.erase(rootId);
}	
}
}




