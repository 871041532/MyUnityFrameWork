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
    public class ABManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ABManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 6, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Awake", _m_Awake);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Start", _m_Start);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetABReferencedCount", _m_GetABReferencedCount);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnloadAsset", _m_UnloadAsset);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAsset", _m_LoadAsset);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetAsync", _m_LoadAssetAsync);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 8, 8);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Log", _m_Log_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgLoadMode", _g_get_CfgLoadMode);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgServerURL", _g_get_CfgServerURL);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgServerPort", _g_get_CfgServerPort);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgManifestAndPlatformName", _g_get_CfgManifestAndPlatformName);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgServerLoadPath", _g_get_CfgServerLoadPath);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgAssetBundleRelativePath", _g_get_CfgAssetBundleRelativePath);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgAssetBundleLoadAbsolutePath", _g_get_CfgAssetBundleLoadAbsolutePath);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "CfgstreamingAssets", _g_get_CfgstreamingAssets);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgLoadMode", _s_set_CfgLoadMode);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgServerURL", _s_set_CfgServerURL);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgServerPort", _s_set_CfgServerPort);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgManifestAndPlatformName", _s_set_CfgManifestAndPlatformName);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgServerLoadPath", _s_set_CfgServerLoadPath);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgAssetBundleRelativePath", _s_set_CfgAssetBundleRelativePath);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgAssetBundleLoadAbsolutePath", _s_set_CfgAssetBundleLoadAbsolutePath);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "CfgstreamingAssets", _s_set_CfgstreamingAssets);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					ABManager gen_ret = new ABManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ABManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Awake(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Awake(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Start(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Start(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetABReferencedCount(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _abName = LuaAPI.lua_tostring(L, 2);
                    
                        int gen_ret = gen_to_be_invoked.GetABReferencedCount( _abName );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadAsset(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    AssetItem _item = (AssetItem)translator.GetObject(L, 2, typeof(AssetItem));
                    
                    gen_to_be_invoked.UnloadAsset( _item );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAsset(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _fullPath = LuaAPI.lua_tostring(L, 2);
                    
                        AssetItem gen_ret = gen_to_be_invoked.LoadAsset( _fullPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetItem>>(L, 3)&& translator.Assignable<System.Action>(L, 4)) 
                {
                    string _fullPath = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetItem> _successCall = translator.GetDelegate<System.Action<AssetItem>>(L, 3);
                    System.Action _failCall = translator.GetDelegate<System.Action>(L, 4);
                    
                    gen_to_be_invoked.LoadAssetAsync( _fullPath, _successCall, _failCall );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetItem>>(L, 3)) 
                {
                    string _fullPath = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetItem> _successCall = translator.GetDelegate<System.Action<AssetItem>>(L, 3);
                    
                    gen_to_be_invoked.LoadAssetAsync( _fullPath, _successCall );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ABManager.LoadAssetAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    LogType _logType;translator.Get(L, 1, out _logType);
                    string _text = LuaAPI.lua_tostring(L, 2);
                    
                    ABManager.Log( _logType, _text );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgLoadMode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, ABManager.CfgLoadMode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgServerURL(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgServerURL);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgServerPort(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgServerPort);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgManifestAndPlatformName(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgManifestAndPlatformName);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgServerLoadPath(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgServerLoadPath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgAssetBundleRelativePath(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgAssetBundleRelativePath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgAssetBundleLoadAbsolutePath(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgAssetBundleLoadAbsolutePath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CfgstreamingAssets(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ABManager.CfgstreamingAssets);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgLoadMode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LoadModeEnum gen_value;translator.Get(L, 1, out gen_value);
				ABManager.CfgLoadMode = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgServerURL(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgServerURL = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgServerPort(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgServerPort = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgManifestAndPlatformName(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgManifestAndPlatformName = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgServerLoadPath(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgServerLoadPath = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgAssetBundleRelativePath(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgAssetBundleRelativePath = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgAssetBundleLoadAbsolutePath(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgAssetBundleLoadAbsolutePath = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CfgstreamingAssets(RealStatePtr L)
        {
		    try {
                
			    ABManager.CfgstreamingAssets = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
