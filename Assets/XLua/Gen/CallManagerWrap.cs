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
    public class CallManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(CallManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 8, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Update", _m_Update);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddInvoke", _m_AddInvoke);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddUpdate", _m_AddUpdate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveUpdate", _m_RemoveUpdate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RegisterEvent", _m_RegisterEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveEvent", _m_RemoveEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerEvent", _m_TriggerEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerAll", _m_TriggerAll);
			
			
			
			
			
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
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					CallManager gen_ret = new CallManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to CallManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Update(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Update(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddInvoke(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Action _call = translator.GetDelegate<System.Action>(L, 2);
                    float _time = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.AddInvoke( _call, _time );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddUpdate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Action _call = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.AddUpdate( _call );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveUpdate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Action _call = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.RemoveUpdate( _call );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)&& translator.Assignable<EventType>(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _call = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    EventType _type;translator.Get(L, 4, out _type);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _call, _type );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _call = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    
                    gen_to_be_invoked.RegisterEvent( _eventName, _call );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to CallManager.RegisterEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)&& translator.Assignable<EventType>(L, 4)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _call = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    EventType _type;translator.Get(L, 4, out _type);
                    
                    gen_to_be_invoked.RemoveEvent( _eventName, _call, _type );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _call = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    
                    gen_to_be_invoked.RemoveEvent( _eventName, _call );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to CallManager.RemoveEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count >= 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<EventType>(L, 3)&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 4) || translator.Assignable<object>(L, 4))) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    EventType _type;translator.Get(L, 3, out _type);
                    object[] _args = translator.GetParams<object>(L, 4);
                    
                    gen_to_be_invoked.TriggerEvent( _eventName, _type, _args );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count >= 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<EventType>(L, 3)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    EventType _type;translator.Get(L, 3, out _type);
                    
                    gen_to_be_invoked.TriggerEvent( _eventName, _type );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count >= 1&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.TriggerEvent( _eventName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to CallManager.TriggerEvent!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerAll(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                CallManager gen_to_be_invoked = (CallManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventName = LuaAPI.lua_tostring(L, 2);
                    object[] _args = translator.GetParams<object>(L, 3);
                    
                    gen_to_be_invoked.TriggerAll( _eventName, _args );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
