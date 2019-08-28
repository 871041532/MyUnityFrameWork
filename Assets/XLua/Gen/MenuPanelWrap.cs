#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class MenuPanelWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(MenuPanel);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 4, 4);
			
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_BtnLoad", _g_get_m_BtnLoad);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_BtnStart", _g_get_m_BtnStart);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_BtnExit", _g_get_m_BtnExit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_ClipAsset", _g_get_m_ClipAsset);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_BtnLoad", _s_set_m_BtnLoad);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_BtnStart", _s_set_m_BtnStart);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_BtnExit", _s_set_m_BtnExit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_ClipAsset", _s_set_m_ClipAsset);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 2 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING))
				{
					string _prefabPath = LuaAPI.lua_tostring(L, 2);
					
					MenuPanel gen_ret = new MenuPanel(_prefabPath);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					MenuPanel gen_ret = new MenuPanel();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to MenuPanel constructor!");
            
        }
        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_BtnLoad(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_BtnLoad);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_BtnStart(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_BtnStart);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_BtnExit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_BtnExit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_ClipAsset(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_ClipAsset);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_BtnLoad(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_BtnLoad = (UnityEngine.UI.Button)translator.GetObject(L, 2, typeof(UnityEngine.UI.Button));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_BtnStart(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_BtnStart = (UnityEngine.UI.Button)translator.GetObject(L, 2, typeof(UnityEngine.UI.Button));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_BtnExit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_BtnExit = (UnityEngine.UI.Button)translator.GetObject(L, 2, typeof(UnityEngine.UI.Button));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_ClipAsset(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                MenuPanel gen_to_be_invoked = (MenuPanel)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_ClipAsset = (AssetItem)translator.GetObject(L, 2, typeof(AssetItem));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
