--1.导入

--2.View或mediator中定义uiCf
--xxx.uiCfg =
--{
--    bg = "bg",  -- str类型是子节点路径
--    listView = {"bg/Panel/total", "XXXXView", },  -- 参数1子节点路径，参数2view类名
--    listView2 = {"xxx/xxx/xxx.prefab", "XXXXView"},  -- 参数1prefab路径，参数2view类名
--    listView3 = {"bg/Panel/total", "XXXXView", {a = "xxx/xxx", b = "xxx/xxx"}}  -- 参数3重定向子类某些节点
--}

--3.在恰当时候比如Start函数中注入
--UIBriefInject.Inject(self)
-- 注入后会自动生成对应的self.xxx属性，定义类型的直接创建好对象，定义prefab路径的加载好之后创建好对象

--4.设置子节点属性
-- 之前操作属性：self.agoHelper.bg:SetButtonClick
-- 现在操作属性：self.bg:SetButtonClick

-- 5.使用子节点创建新view
-- 之前做法
--local XXXXView = require("XXXXView")
--local listView = self:_GetProxyUI():CreateUI(XXXXView, self.agoHelper.listView.gameObject)
-- 现在根据定配置直接生成view对象
--self.listView  -- 这个就是XXXXView对象

-- 6.加载prefab创建一个子节点
--之前做法
--local XXXXView = require("XXXXView")
--local listView = self:_GetProxyUI():_CreateUIByPath(XXXXView, "xxx.xxx.prefab")
--现在做法
--uiCfg中定义了以.prefab结尾的路径，会自动load并创建
--self.listView2  -- 这个就是XXXXView对象

-- 7.属性判空
-- 之前用agoHelper子节点不存在空其实会报错，如self.agoHelper.listView.gameObject == nil
-- 现在直接判空：self.listView == nil

-- 7.在view或mediator中操作自己
-- 之前是self.agoHelper.__Self:SetVisible(false)
-- 现在是self.me:SetVisible(false)

-- 8.页面打开后某个时机创建一个或多个页面
-- self:AppendUI(key, "xxx/xxx.prefab")、self:AppendUI(key, {"xxx/xxx.prefab", "XXXXView"})

-- 9.使用别的view，除了子节点路径其余都一样
-- 配置中第三项是重定向子节点的某些属性的路径，要求子节点也使用uiCfg方式
-- 限制是只能绑一些不在子节点 _OnInit 时使用的属性

---@class UIBriefInject
local UIBriefInject = {}

local getCom = function(self, comName)
    if not self.__name2Com then
        self.__name2Com = {}
    end
    local com
    if self.__name2Com[comName] then
        com = self.__name2Com[comName]
    else
        com = self.gameObject:GetComponent(comName)
        self.__name2Com[comName] = com
    end
    return com
end

---@class funcMap
local funcMap = {}

-- 获取transform尺寸
funcMap.GetSize = function(self)
    return self.transform.rect.size
end

-- 设置text文本
funcMap.SetTextString = function(self, text)
    local com = getCom(self, "Text")
    if not com then return end
    com.text = text
end

-- 获取text文本
funcMap.GetTextString = function(self)
    local com = getCom(self, "Text")
    if not com then return end
    return com.text
end

-- 设置text多语言文本
funcMap.SetMultiTextString = function(self, key, ...)
    local com = getCom(self, "Text")
    if not com then return end
    local text = ugui.getText(key, ...)
    com.text = text
end

-- 设置图片sprite
funcMap.SetSpriteByPath = function(self, loader, path)
    local com = getCom(self, "Image")
    if not com then return end
    local sprite = loader:LoadSprite(path)
    com.sprite = sprite
end

-- 设置自身可见性
funcMap.SetVisible = function(self, visible)
    self.gameObject:SetActive(visible)
end

-- 获取自身可见性
funcMap.IsVisible = function(self)
    return self.gameObject.activeSelf
end

-- 设置Button灰态
funcMap.SetButtonInteractable = function(self, b)
    local com = getCom(self, "Button")
    if not com then return end
	com.interactable = b
end

-- 临时追加UI(oneCfg就是uiCfg中的一项值)
funcMap.AppendUI = function(self, key, oneCfg)
    local cfg = {}
    cfg[key] = oneCfg
    UIBriefInject._InjectUICfg(self, cfg)
end

-- 设置Button点击事件
funcMap.SetButtonClick = function(self, callback, duration, soundEffectName)
    local com = getCom(self, "Button")
    if not com then return end
	ugui.SetButtonClick(com, callback, duration, soundEffectName)
end

-- 设置NN4RepeatButton间隔时间
funcMap.SetRepeatButtonIntervalTime = function(self, time)
    local com = getCom(self, "NN4RepeatButton")
    if not com then return end
    com.intervalTime = time
end

-- 设置NN4RepeatButton重复时间
funcMap.SetRepeatButtonRepeatTime = function(self, time)
    local com = getCom(self, "NN4RepeatButton")
    if not com then return end
    com.repeatTime = time
end

-- 设置NN4RepeatButton长按按下call
funcMap.SetRepeatButtonPress = function(self, call)
    local com = getCom(self, "NN4RepeatButton")
    if not com then return end
    ugui.SetNN4RepeatButtonPress(com, call)
end

-- 设置NN4RepeatButton短按call
funcMap.SetRepeatButtonRelease = function(self, call)
    local com = getCom(self, "NN4RepeatButton")
    if not com then return end
    ugui.SetNN4RepeatButtonRelease(com, call)
end

-- 设置NN4RepeatButton长按抬起call
funcMap.SetRepeatButtonRepeatRelease = function(self, call)
    local com = getCom(self, "NN4RepeatButton")
    if not com then return end
    ugui.SetNN4RepeatButtonRepeatRelease(com, call)
end

-- 自身transformlocalMove
funcMap.DOLocalMove = function(self, localPos, time, loop, doneCall)
	if doneCall then
		self.transform:DOLocalMove(localPos, time, loop):OnComplete(doneCall)
	else
		self.transform:DOLocalMove(localPos, time, loop)
	end
end

-- 自身Animator播放动画
funcMap.PlayAnim = function(self, name, layer, normalizedTime)
    local com = getCom(self, "Animator")
    if not com then return end
	com:Play(name, layer, normalizedTime)
end

-- 自身Animator触发事件
funcMap.SetAnimatorTrigger = function(self, triggerName)
    local com = getCom(self, "Animator")
    if not com then return end
    com:SetTrigger(triggerName)
end

-- 设置Animator关键帧call
funcMap.SetAnimCallback = function(self, name, func)
    local com = getCom(self, "NN4AnimationEvent")
    if not com then return end
	com:SetAnimationCallback(name, func)
end

-- 设置进度条value
funcMap.SetProgress = function(self, value)
    local com = getCom(self, "Slider")
    if not com then return end
	com.value = value
end

 -- 进度调带动画value
funcMap.DoProgress = function(self, value, time, doneCall)
    local com = getCom(self, "Slider")
    if not com then return end
	if doneCall then
		com:DOValue(value, time):OnComplete(doneCall)
	else
		com:DOValue(value, time)
	end
end

-- 获取进度条进度
funcMap.GetProgress = function(self)
    local com = getCom(self, "Slider")
    if not com then return end
	return com.value
end

-- 设置toggle开关
funcMap.SetToggleValue = function(self, b)
    local com = getCom(self, "Toggle")
    if not com then return end
	com.isOn = b
end

-- 设置toggle值变化回调
funcMap.SetToggleValueChanged = function(self, func)
    local com = getCom(self, "Toggle")
    if not com then return end
	com.onValueChanged:AddListener(function()
		func(com.isOn)
	end)
end


-- s1:UIBase里面没有transform，加一下
UIBriefInject._InjectAttribute = function(item)
    if (not item.transform) and item.gameObject then
        item.transform = item.gameObject.transform
    end
end

-- s2:针对UIBase等采用安全注入funcMap方式
UIBriefInject._InjectFuncMap = function(item, isInsert)
    if isInsert then
        for k, v in pairs(funcMap) do
        -- 直接覆盖最安全，判空的话父类加入同名方法会影响到子类
        item[k] = v
        end
    end
    -- funcMap遍历注册比较耗，改用self.me方式
    item.me = UIBriefInject._CreateBriefCom(item.transform)
end

-- s3:针对无UIBase和UIView的最终节点，使用元表继承方式
---@class BriefUICom:funcMap
local BriefUICom = class("BriefUICom", funcMap)
function BriefUICom:ctor(transform)
    self.transform = transform
    self.gameObject = transform.gameObject
end
UIBriefInject._CreateBriefCom = function(transform)
    local obj = BriefUICom.New(transform)
    return obj
end

-- s4:解析self.uiCfg，自动安全地生成属性和子节点，uiConfig优先使用，为空使用self.uiCfg
local GetChild = function(self, childPath)
    local _, last = string.find(childPath,".prefab", string.len(childPath) - 6)
    local go
    if last then
        -- 动态创建
        go = unity.LoadPrefabInstantiate(childPath, self.transform)
        go = go.transform
    else
        -- 节点查找
        if childPath == "" then
            go = self.transform  -- 为自己添加com时路径配置为 ""
        else
            go = self.transform:Find(childPath)
        end
    end
    return go
end
UIBriefInject._InjectUICfg = function(self, uiConfig)
     local uiCfg = uiConfig or self.uiCfg
     if uiCfg then
        for k, v in pairs(uiCfg) do
            local key
            local classType
            local child
            local childRedictCfg
            key = k
            if type(v) == "string" then
                child = GetChild(self, v)
            elseif type(v) == "table" then
                child = GetChild(self, v[1])
                if v[2] then
                    classType = require(v[2])
                end
                childRedictCfg = v[3]
            end
            if child then
                local childUI
                if classType then
                    if self._GetProxyUI then
                        childUI = self:_GetProxyUI():CreateUI(classType, child)
                    else
                        log.error("uiCfg配置错误, 在无ProxyUI的类中指定了类型", self.__cname)
                    end
                else
                    childUI = UIBriefInject._CreateBriefCom(child)
                end
                 self[key] = childUI
                if childRedictCfg then
                    UIBriefInject._InjectUICfg(childUI, childRedictCfg)
                end
            end
        end
    end
end

---------------------------------------- 外部接口 ---------------------------------------------------
-- 一条龙
-- uiCfg优先使用, 可空，空则使用self.uiCfg。如果self.uiCfg也为空则不注入
-- isInsert为true则将funcMap插入到self中
UIBriefInject.Inject = function(self, uiCfg, isInsert)
    if not self then
        return
    end
    UIBriefInject._InjectAttribute(self)
    UIBriefInject._InjectFuncMap(self, isInsert)
    UIBriefInject._InjectUICfg(self, uiCfg)
end
-----------------------------------------------------------------------------------------------------
return UIBriefInject