

using System;

static class BTGlobal
{
	public const int BT_MaxBTChildNodeNum = 16;
	public const int BT_InvalidChildNodeIndex = BT_MaxBTChildNodeNum;
	public static bool RECURSION_OK = true;
}

enum E_ParallelFinishCondition
{
	Or,
	And
};

enum StatusBTRunning
{
	Executing,
	Finish,
	ErrorTransition = -1,
};

enum StausNodeTerminal
{
	Ready,
	Running,
	Finish,
};


class BTPrecondition
{
	public virtual bool ExternalCondition()
	{
		return m_dynamicJudge == null || m_dynamicJudge();
	}
	
	public void SetDynamicJudge(Func<bool> call)
	{
		m_dynamicJudge = call;
	}
	
	private Func<bool> m_dynamicJudge = null;
};

class BTPreconditionAnd :  BTPrecondition
{

public BTPreconditionAnd(BTPrecondition lhs, BTPrecondition rhs)
{
	m_lhs = lhs;
	m_rhs = rhs;
}

~BTPreconditionAnd() {
}
	
public override bool ExternalCondition()
{
	return m_lhs.ExternalCondition() && m_rhs.ExternalCondition();
}

private readonly BTPrecondition m_lhs = null;
private readonly BTPrecondition m_rhs = null;
};

class BTPreconditionOr: BTPrecondition
{

	public BTPreconditionOr(BTPrecondition lhs, BTPrecondition rhs)
	{
		m_lhs = lhs;
		m_rhs = rhs;
	}

	public override bool ExternalCondition()  
	{
		return m_lhs.ExternalCondition() || m_rhs.ExternalCondition();
	}

private readonly BTPrecondition m_lhs = null;
private readonly BTPrecondition m_rhs = null;
};

class BTPreconditionXor : public BTPrecondition
{
public:
	BTPreconditionXor(BTPrecondition* lhs, BTPrecondition* rhs)
		: m_lhs(lhs)
		, m_rhs(rhs)
	{
		D_CHECK(m_lhs && m_rhs);
	}
	~BTPreconditionXor() {
		D_SafeDelete(m_lhs);
		D_SafeDelete(m_rhs);
	}
	virtual bool ExternalCondition(const BTNodeInputParam& input) const {
		return m_lhs->ExternalCondition(input) ^ m_rhs->ExternalCondition(input);
	}
private:
	BTPrecondition* m_lhs;
	BTPrecondition* m_rhs;
};

class BTNode
{
public:
	BTNode(BTNode* parentNode)
	{
		for (int i = 0; i < BT_MaxBTChildNodeNum; ++i)
			m_childNodeList[i] = nullptr;

		_SetParentNode(parentNode);
	}
	virtual ~BTNode()
	{
		for (unsigned int i = 0; i < m_ChildNodeCount; ++i)
		{
			D_SafeDelete(m_childNodeList[i]);
		}
	}
	void SetPreCondition(std::function<bool()> precondition)
	{
		m_precondition = precondition;
	}
	bool Evaluate(const BTNodeInputParam& input)
	{
		return (m_precondition == nullptr || m_precondition()) && OnEvaluate(input);
	}
	void Transition(const BTNodeInputParam& input)
	{
		OnTransition(input);
	}
	StatusBTRunning Tick(const BTNodeInputParam& input, BTNodeOutputParam& output)
	{
		return OnTick(input, output);
	}
	//---------------------------------------------------------------
	BTNode& AddChildNode(BTNode* childNode)
	{
		if (m_ChildNodeCount == BT_MaxBTChildNodeNum)
		{
			D_Output("The number of child BTNodes is up to %d.", BT_MaxBTChildNodeNum);
			D_CHECK(0);
			return (*this);
		}
		m_childNodeList[m_ChildNodeCount] = childNode;
		++m_ChildNodeCount;
		return (*this);
	}
	BTNode& SetDebugName(const char* debugName)
	{
		m_debugName = debugName;
		return (*this);
	}
	const BTNode* GetLastActiveNode() const
	{
		return m_lastActiveNode;
	}
	void SetActiveNode(BTNode* node)
	{
		m_lastActiveNode = m_activeNode;
		m_activeNode = node;
		if (m_parentNode != nullptr)
			m_parentNode->SetActiveNode(node);
	}
	const char* GetDebugName() const
	{
		return m_debugName.c_str();
	}
protected:
	//--------------------------------------------------------------
	// virtual function
	//--------------------------------------------------------------
	virtual bool OnEvaluate(const BTNodeInputParam& input)
	{ 
		return true;
	}
	virtual void OnTransition(const BTNodeInputParam& input)
	{
	}
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
	{
		return StatusBTRunning::Finish;
	}
protected:
	void _SetParentNode(BTNode* parentNode)
	{
		m_parentNode = parentNode;
	}
	bool _bCheckIndex(uint index) const
	{
		return index >= 0 && index < m_ChildNodeCount;
	}
protected:
	BTNode*                m_childNodeList[BT_MaxBTChildNodeNum];
	readonly uint						  m_ChildNodeCount = 0;
	BTNode*                m_parentNode = nullptr;
	BTNode*                m_activeNode = nullptr;
	BTNode*				   m_lastActiveNode = nullptr;
	std::function<bool()> m_precondition = nullptr;
	std::
	readonly string				  m_debugName = "defaultNodeName";
};

class BTNodePrioritySelector : public BTNode
{
public:
	BTNodePrioritySelector(BTNode* parentNode)
		: BTNode(parentNode){}
	virtual bool OnEvaluate(const BTNodeInputParam& input);
	virtual void OnTransition(const BTNodeInputParam& input);
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output);

protected:
	uint m_currentSelectIndex = BT_InvalidChildNodeIndex;
	uint m_lastSelectIndex = BT_InvalidChildNodeIndex;
};
bool BTNodePrioritySelector::OnEvaluate(const BTNodeInputParam& input)
{
	m_currentSelectIndex = BT_InvalidChildNodeIndex;
	for (uint i = 0; i < m_ChildNodeCount; ++i)
	{
		BTNode* child = m_childNodeList[i];
		if (child->Evaluate(input))
		{
			m_currentSelectIndex = i;
			return true;
		}
	}
	return false;
}
void BTNodePrioritySelector::OnTransition(const BTNodeInputParam& input)
{
	if (_bCheckIndex(m_currentSelectIndex))
	{
		BTNode* child = m_childNodeList[m_currentSelectIndex];
		child->Transition(input);
	}
	m_lastSelectIndex = BT_InvalidChildNodeIndex;
}
StatusBTRunning BTNodePrioritySelector::OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
{
	StatusBTRunning bIsFinish = StatusBTRunning::Finish;
	// ��cur_index��last_index��ѡ�����õģ���ֵ��last_index
	if (_bCheckIndex(m_currentSelectIndex))
	{
		if (m_lastSelectIndex != m_currentSelectIndex)  //new select result
		{
			if (_bCheckIndex(m_lastSelectIndex))
			{
				BTNode* child = m_childNodeList[m_lastSelectIndex];
				child->Transition(input);   //we need transition
			}
			m_lastSelectIndex = m_currentSelectIndex;
		}
	}
	if (_bCheckIndex(m_lastSelectIndex))
	{
		//Running node
		BTNode* child = m_childNodeList[m_lastSelectIndex];
		bIsFinish = child->Tick(input, output);
		//clear variable if finish
		if (bIsFinish == StatusBTRunning::Finish)
			m_lastSelectIndex = BT_InvalidChildNodeIndex;
	}
	return bIsFinish;
}

// ��Evaluateʱ���precondition���˾���Ϊ��true
// ����ӽڵ�Evaluate��������ô��tickʱ��ִ��
class BTNodeNoRecursionPrioritySelector : public BTNodePrioritySelector
{
public:
	BTNodeNoRecursionPrioritySelector(BTNode* parentNode) : BTNodePrioritySelector(parentNode) {}
	virtual bool OnEvaluate(const BTNodeInputParam& input) final
	{
		bool ret = BTNodePrioritySelector::OnEvaluate(input);
		if (m_precondition != nullptr)
		{
			// �ߵ�����֤��ǰ�������Ѿ�ͨ��������Ҫ�ӽڵ�ĵݹ��������ж�
			if (!ret)
			{
				// ����ӽڵ�evaluateû������ô���������tick��ִ��
				BTGlobal::RECURSION_OK = false;
			}
			ret = true;
		}
		return ret;
	}
};

class BTNodeNonePrioritySelector : public BTNodePrioritySelector
{
public:
	BTNodeNonePrioritySelector(BTNode* parentNode)
		: BTNodePrioritySelector(parentNode){}
	virtual bool OnEvaluate(const BTNodeInputParam& input);
};
bool BTNodeNonePrioritySelector::OnEvaluate(const BTNodeInputParam& input)
{
	if (_bCheckIndex(m_currentSelectIndex))
	{
		BTNode* child = m_childNodeList[m_currentSelectIndex];
		if (child->Evaluate(input))
		{
			return true;
		}
	}
	return BTNodePrioritySelector::OnEvaluate(input);
}

class BTNodeSequence : public BTNode
{
public:
	BTNodeSequence(BTNode* parentNode)
		: BTNode(parentNode){}
	virtual bool OnEvaluate(const BTNodeInputParam& input);
	virtual void OnTransition(const BTNodeInputParam& input);
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output);

private:
	uint m_currentNodeIndex = BT_InvalidChildNodeIndex;
};
bool BTNodeSequence::OnEvaluate(const BTNodeInputParam& input)
{
	uint testNode;
	if (m_currentNodeIndex == BT_InvalidChildNodeIndex)
		testNode = 0;
	else
		testNode = m_currentNodeIndex;
		
	if (_bCheckIndex(testNode))
	{
		BTNode* child = m_childNodeList[testNode];
		if (child->Evaluate(input))
			return true;
	}
	return false;
}
void BTNodeSequence::OnTransition(const BTNodeInputParam& input)
{
	if (_bCheckIndex(m_currentNodeIndex))
	{
		BTNode* child = m_childNodeList[m_currentNodeIndex];
		child->Transition(input);
	}
	m_currentNodeIndex = BT_InvalidChildNodeIndex;
}
StatusBTRunning BTNodeSequence::OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
{
	StatusBTRunning bIsFinish = StatusBTRunning::Finish;

	//First Time
	if (m_currentNodeIndex == BT_InvalidChildNodeIndex)
		m_currentNodeIndex = 0;

	BTNode* child = m_childNodeList[m_currentNodeIndex];
	if (child != nullptr)
	{
		bIsFinish = child->Tick(input, output);
	}
	if (bIsFinish == StatusBTRunning::Finish)
	{
		++m_currentNodeIndex;
		//sequence is over
		if (m_currentNodeIndex == m_ChildNodeCount)
		{
			m_currentNodeIndex = BT_InvalidChildNodeIndex;
		}
		else
		{
			bIsFinish = StatusBTRunning::Executing;
		}
	}
	if (bIsFinish == StatusBTRunning::ErrorTransition)
	{
		m_currentNodeIndex = BT_InvalidChildNodeIndex;
	}
	return bIsFinish;
}

class BTNodeTerminal : public BTNode
{
public:
	BTNodeTerminal(BTNode* parentNode)
		: BTNode(parentNode){}
	BTNodeTerminal& SetDynamicOnExecute(std::function<StatusBTRunning(const BTNodeInputParam& input, BTNodeOutputParam& output)> call)
	{
		m_dynamicOnExecute = call;
		return (*this);
	}
	BTNodeTerminal& SetDynamicOnEnter(std::function<void(const BTNodeInputParam&)> call)
	{
		m_dynamicOnEnter = call;
		return (*this);
	}
	BTNodeTerminal& SetDynamicOnExit(std::function<void(const BTNodeInputParam&, StatusBTRunning)> call)
	{
		m_dynamicOnExit = call;
		return (*this);
	}
	virtual void OnTransition(const BTNodeInputParam& input) override;
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output) override;

protected:
	virtual bool OnEvaluate(const BTNodeInputParam& input) final
	{
		return true;
	}
	virtual void OnEnter(const BTNodeInputParam& input) 
	{
		if (m_dynamicOnEnter != nullptr)
		{
			m_dynamicOnEnter(input);
		}
	}
	virtual StatusBTRunning	OnExecute(const BTNodeInputParam& input, BTNodeOutputParam& output)
	{ 
		StatusBTRunning returnStatus = StatusBTRunning::Finish;
		if (m_dynamicOnExecute != nullptr)
		{
			returnStatus = m_dynamicOnExecute(input, output);
		}
		return returnStatus;
	}
	virtual void OnExit(const BTNodeInputParam& input, StatusBTRunning _ui_ExitID) 
	{
		if (m_dynamicOnExit != nullptr)
		{
			m_dynamicOnExit(input, _ui_ExitID);
		}
	}

private:
	StausNodeTerminal m_Status = StausNodeTerminal::Ready;
	bool m_needExit = false;
	// ��̬���õ�OnExecute����
	std::function<StatusBTRunning(const BTNodeInputParam&, BTNodeOutputParam&)> m_dynamicOnExecute = nullptr;
	// ��̬���õ�OnEnter����
	std::function<void(const BTNodeInputParam&)> m_dynamicOnEnter = nullptr;
	// ��̬���õ�OnExit����
	std::function<void(const BTNodeInputParam&, StatusBTRunning)> m_dynamicOnExit = nullptr;

};
void BTNodeTerminal::OnTransition(const BTNodeInputParam& input)
{
	if (m_needExit)     //call Exit if we have called Enter
		OnExit(input, StatusBTRunning::ErrorTransition);

	SetActiveNode(nullptr);
	m_Status = StausNodeTerminal::Ready;
	m_needExit = false;
}
StatusBTRunning BTNodeTerminal::OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
{
	StatusBTRunning bIsFinish = StatusBTRunning::Finish;

	if (m_Status == StausNodeTerminal::Ready)
	{
		OnEnter(input);
		m_needExit = true;
		m_Status = StausNodeTerminal::Running;
		SetActiveNode(this);
	}
	if (m_Status == StausNodeTerminal::Running)
	{
		bIsFinish = OnExecute(input, output);
		SetActiveNode(this);
		if (bIsFinish == StatusBTRunning::Finish || bIsFinish == StatusBTRunning::ErrorTransition)
			m_Status = StausNodeTerminal::Finish;
	}
	if (m_Status == StausNodeTerminal::Finish)
	{
		if (m_needExit)     //call Exit if we have called Enter
			OnExit(input, bIsFinish);
		m_Status = StausNodeTerminal::Ready;
		m_needExit = false;
		SetActiveNode(NULL);
		return bIsFinish;
	}
	return bIsFinish;
}

class BTNodeParallel : public BTNode
{
public:
	BTNodeParallel(BTNode* parentNode)
		: BTNode(parentNode)
	{
		for (uint i = 0; i < BT_MaxBTChildNodeNum; ++i)
			m_childNodeStatus[i] = StatusBTRunning::Executing;
	}
	virtual bool OnEvaluate(const BTNodeInputParam& input);
	virtual void OnTransition(const BTNodeInputParam& input);
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output);
	BTNodeParallel& SetFinishCondition(E_ParallelFinishCondition condition);
private:
	E_ParallelFinishCondition m_finishCondition = E_ParallelFinishCondition::Or;
	StatusBTRunning		  m_childNodeStatus[BT_MaxBTChildNodeNum];
};
bool BTNodeParallel::OnEvaluate(const BTNodeInputParam& input)
{
	for (uint i = 0; i < m_ChildNodeCount; ++i)
	{
		BTNode* child = m_childNodeList[i];
		if (m_childNodeStatus[i] == StatusBTRunning::Executing)
		{
			if (!child->Evaluate(input))
			{
				return false;
			}
		}
	}
	// ֻҪ��һ���ӽڵ㲻ͨ���򷵻�false��ȫ��ͨ������true
	return true;
}
void BTNodeParallel::OnTransition(const BTNodeInputParam& input)
{
	for (uint i = 0; i < BT_MaxBTChildNodeNum; ++i)
		m_childNodeStatus[i] = StatusBTRunning::Executing;

	for (uint i = 0; i < m_ChildNodeCount; ++i)
	{
		BTNode* child = m_childNodeList[i];
		child->Transition(input);
	}
}
BTNodeParallel& BTNodeParallel::SetFinishCondition(E_ParallelFinishCondition condition)
{
	m_finishCondition = condition;
	return (*this);
}
StatusBTRunning BTNodeParallel::OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
{
	unsigned int finishedChildCount = 0;
	for (unsigned int i = 0; i < m_ChildNodeCount; ++i)
	{
		BTNode* oBN = m_childNodeList[i];
		if (m_finishCondition == E_ParallelFinishCondition::Or)
		{
			if (m_childNodeStatus[i] == StatusBTRunning::Executing)
			{
				m_childNodeStatus[i] = oBN->Tick(input, output);
			}
			if (m_childNodeStatus[i] != StatusBTRunning::Executing)
			{
				for (uint i = 0; i < BT_MaxBTChildNodeNum; ++i)
					m_childNodeStatus[i] = StatusBTRunning::Executing;
				return StatusBTRunning::Finish;
			}
		}
		else if (m_finishCondition == E_ParallelFinishCondition::And)
		{
			if (m_childNodeStatus[i] == StatusBTRunning::Executing)
			{
				m_childNodeStatus[i] = oBN->Tick(input, output);
			}
			if (m_childNodeStatus[i] != StatusBTRunning::Executing)
			{
				finishedChildCount++;
			}
		}
		else
		{
			D_CHECK(0);
		}
	}
	if (finishedChildCount == m_ChildNodeCount)
	{
		for (uint i = 0; i < BT_MaxBTChildNodeNum; ++i)
			m_childNodeStatus[i] = StatusBTRunning::Executing;
		return StatusBTRunning::Finish;
	}
	return StatusBTRunning::Executing;
}


class BTNodeLoop : public BTNode
{
public:
	static const int kInfiniteLoop = -1;
public:
	BTNodeLoop(BTNode* parentNode, int loopCount = kInfiniteLoop)
		: BTNode(parentNode)
		, m_loopCount(loopCount)
	{}
	virtual bool OnEvaluate(const BTNodeInputParam& input);
	virtual void OnTransition(const BTNodeInputParam& input);
	virtual StatusBTRunning OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output);
private:
	int m_loopCount;
	int m_currentCount = 0;
};
bool BTNodeLoop::OnEvaluate(const BTNodeInputParam& input)
{
	bool checkLoopCount = (m_loopCount == kInfiniteLoop) ||
		m_currentCount < m_loopCount;

	if (!checkLoopCount)
		return false;

	if (_bCheckIndex(0))
	{
		BTNode* child = m_childNodeList[0];
		if (child->Evaluate(input))
			return true;
	}
	return false;
}
void BTNodeLoop::OnTransition(const BTNodeInputParam& input)
{
	if (_bCheckIndex(0))
	{
		BTNode* child = m_childNodeList[0];
		child->Transition(input);
	}
	m_currentCount = 0;
}
StatusBTRunning BTNodeLoop::OnTick(const BTNodeInputParam& input, BTNodeOutputParam& output)
{
	StatusBTRunning bIsFinish = StatusBTRunning::Finish;
	if (_bCheckIndex(0))
	{
		BTNode* oBN = m_childNodeList[0];
		bIsFinish = oBN->Tick(input, output);

		if (bIsFinish == StatusBTRunning::Finish)
		{
			if (m_loopCount != kInfiniteLoop)
			{
				// ����ѭ��
				++m_currentCount;
				if (m_currentCount < m_loopCount)  // ����ԭ���� == 
				{
					bIsFinish = StatusBTRunning::Executing;
				}
			}
			else
			{
				// ����ѭ��
				bIsFinish = StatusBTRunning::Executing;
			}
		}
	}
	if (bIsFinish != StatusBTRunning::Executing)
	{
		m_currentCount = 0;
	}
	return bIsFinish;
}

class BTNodeFactory
{
public:
	static BTNode& CreateParallelNode(BTNode* parent, E_ParallelFinishCondition condition, const char* debugName)
	{
		BTNodeParallel* pReturn = new BTNodeParallel(parent);
		pReturn->SetFinishCondition(condition);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
	static BTNode& CreatePrioritySelectorNode(BTNode* parent, const char* debugName)
	{
		BTNodePrioritySelector* pReturn = new BTNodePrioritySelector(parent);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
	static BTNode& CreateNonePrioritySelectorNode(BTNode* parent, const char* debugName)
	{
		BTNodeNonePrioritySelector* pReturn = new BTNodeNonePrioritySelector(parent);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
	static BTNode& CreateSequenceNode(BTNode* parent, const char* debugName)
	{
		BTNodeSequence* pReturn = new BTNodeSequence(parent);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
	static BTNode& CreateLoopNode(BTNode* parent, const char* debugName, int loopCount)
	{
		BTNodeLoop* pReturn = new BTNodeLoop(parent, loopCount);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
	template<typename T>
	static BTNodeTerminal& CreateTeminalNode(BTNode* parent, const char* debugName)
	{
		BTNodeTerminal* pReturn = new T(parent);
		CreateNodeCommon(pReturn, parent, debugName);
		return (*pReturn);
	}
private:
	static void CreateNodeCommon(BTNode* me, BTNode* parent, const char* debugName)
	{
		if (parent)
			parent->AddChildNode(me);
		me->SetDebugName(debugName);
	}
};