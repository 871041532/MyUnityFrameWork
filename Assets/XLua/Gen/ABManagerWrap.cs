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
			Utils.BeginObjectRegister(type, L, translator, 0, 9, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Awake", _m_Awake);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetGameObject", _m_LoadAssetGameObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadScene", _m_LoadScene);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadSceneAsync", _m_LoadSceneAsync);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetBundleByAssetName", _m_LoadAssetBundleByAssetName);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetBundleByAssetNameAsync", _m_LoadAssetBundleByAssetNameAsync);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetBundle", _m_LoadAssetBundle);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetBundleAsync", _m_LoadAssetBundleAsync);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnDestroy", _m_OnDestroy);
			
			
			
			
			
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
        static int _m_LoadAssetGameObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _fullPath = LuaAPI.lua_tostring(L, 2);
                    
                        UnityEngine.GameObject gen_ret = gen_to_be_invoked.LoadAssetGameObject( _fullPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadScene(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.LoadScene( _sceneName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadSceneAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _sceneName = LuaAPI.lua_tostring(L, 2);
                    System.Action _successCall = translator.GetDelegate<System.Action>(L, 3);
                    System.Action _failCall = translator.GetDelegate<System.Action>(L, 4);
                    
                    gen_to_be_invoked.LoadSceneAsync( _sceneName, _successCall, _failCall );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetBundleByAssetName(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _assetFullPath = LuaAPI.lua_tostring(L, 2);
                    
                        AssetBundleItem gen_ret = gen_to_be_invoked.LoadAssetBundleByAssetName( _assetFullPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetBundleByAssetNameAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetBundleItem>>(L, 3)&& translator.Assignable<System.Action>(L, 4)) 
                {
                    string _assetFullPath = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetBundleItem> _successCall = translator.GetDelegate<System.Action<AssetBundleItem>>(L, 3);
                    System.Action _failCall = translator.GetDelegate<System.Action>(L, 4);
                    
                    gen_to_be_invoked.LoadAssetBundleByAssetNameAsync( _assetFullPath, _successCall, _failCall );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetBundleItem>>(L, 3)) 
                {
                    string _assetFullPath = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetBundleItem> _successCall = translator.GetDelegate<System.Action<AssetBundleItem>>(L, 3);
                    
                    gen_to_be_invoked.LoadAssetBundleByAssetNameAsync( _assetFullPath, _successCall );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ABManager.LoadAssetBundleByAssetNameAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetBundle(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _abName = LuaAPI.lua_tostring(L, 2);
                    
                        AssetBundleItem gen_ret = gen_to_be_invoked.LoadAssetBundle( _abName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetBundleAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetBundleItem>>(L, 3)&& translator.Assignable<System.Action>(L, 4)) 
                {
                    string _abName = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetBundleItem> _successCall = translator.GetDelegate<System.Action<AssetBundleItem>>(L, 3);
                    System.Action _failCall = translator.GetDelegate<System.Action>(L, 4);
                    
                    gen_to_be_invoked.LoadAssetBundleAsync( _abName, _successCall, _failCall );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<AssetBundleItem>>(L, 3)) 
                {
                    string _abName = LuaAPI.lua_tostring(L, 2);
                    System.Action<AssetBundleItem> _successCall = translator.GetDelegate<System.Action<AssetBundleItem>>(L, 3);
                    
                    gen_to_be_invoked.LoadAssetBundleAsync( _abName, _successCall );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ABManager.LoadAssetBundleAsync!");
            
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
        static int _m_OnDestroy(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ABManager gen_to_be_invoked = (ABManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnDestroy(  );
                    
                    
                    
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
