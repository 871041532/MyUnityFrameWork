
-- 定义名为 Shape 的基础类  
local menu = Class("NewMenu")

function menu:ctor() 
   UIMgr:RegisterWindow('NewMenu', function() 
		return CS.MenuPanel()
	end)
   self.cs_dispose_call = function (...)
   	self:dispose()
   end
   CallMgr:RegisterEvent('OnDestroy', self.cs_dispose_call)
   self.cs_win = UIMgr:GetOrCreateWindow('NewMenu')
end

function menu:show()
	self.cs_win:Show()
end

function menu:dispose()
	CallMgr:RemoveEvent('OnDestroy', self.cs_dispose_call)
	-- UIMgr:DestroyWindow(self.cs_win)
	UIMgr:UnRegisterWindow('NewMenu')
end 

return menu