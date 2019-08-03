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
    public class GameManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(GameManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 2, 6, 6);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Log", _m_Log);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnGUI", _m_OnGUI);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_LuaMgr", _g_get_m_LuaMgr);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_ABMgr", _g_get_m_ABMgr);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_CutSceneMgr", _g_get_m_CutSceneMgr);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_ResMgr", _g_get_m_ResMgr);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_ObjectMgr", _g_get_m_ObjectMgr);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "m_log_object", _g_get_m_log_object);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_LuaMgr", _s_set_m_LuaMgr);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_ABMgr", _s_set_m_ABMgr);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_CutSceneMgr", _s_set_m_CutSceneMgr);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_ResMgr", _s_set_m_ResMgr);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_ObjectMgr", _s_set_m_ObjectMgr);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "m_log_object", _s_set_m_log_object);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 1, 0);
			
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Instance", _g_get_Instance);
            
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					GameManager gen_ret = new GameManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to GameManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _line = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.Log( _line );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnGUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnGUI(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, GameManager.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_LuaMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_LuaMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_ABMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_ABMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_CutSceneMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_CutSceneMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_ResMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_ResMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_ObjectMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_ObjectMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_m_log_object(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.m_log_object);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_LuaMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_LuaMgr = (LuaManager)translator.GetObject(L, 2, typeof(LuaManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_ABMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_ABMgr = (ABManager)translator.GetObject(L, 2, typeof(ABManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_CutSceneMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_CutSceneMgr = (CutSceneManager)translator.GetObject(L, 2, typeof(CutSceneManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_ResMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_ResMgr = (ResourceManager)translator.GetObject(L, 2, typeof(ResourceManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_ObjectMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_ObjectMgr = (ObjectManager)translator.GetObject(L, 2, typeof(ObjectManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_m_log_object(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameManager gen_to_be_invoked = (GameManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.m_log_object = (UnityEngine.UI.Text)translator.GetObject(L, 2, typeof(UnityEngine.UI.Text));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
