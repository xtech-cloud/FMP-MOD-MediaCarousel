
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.83.0.  DO NOT EDIT!
//*************************************************************************************

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.MediaCarousel.LIB.Bridge;
using XTC.FMP.MOD.MediaCarousel.LIB.MVCS;
using XTC.FMP.MOD.MediaCarousel.LIB.Proto;

namespace XTC.FMP.MOD.MediaCarousel.LIB.Unity
{
    public class MyInstanceBase
    {
        public string uid { get; private set; }
        public GameObject rootUI { get; private set; }
        public GameObject rootWorld { get; private set; }
        public GameObject rootAttachments { get; private set; }

        /// <summary>
        /// 内容对象池，管理从内容目录中加载到内存中的对象
        /// </summary>
        /// <remarks>
        /// 在实例打开(Open)时准备，在实例关闭(Close)时清理
        /// </remarks>
        public ObjectsPool contentObjectsPool { get; private set; }

        /// <summary>
        /// 主题对象池，管理从主题目录加载到内存中的对象
        /// </summary>
        /// <remarks>
        /// 在实例创建(Create)时准备，在实例删除(Delete)时清理
        /// </remarks>
        public ObjectsPool themeObjectsPool { get; private set; }

        /// <summary>
        /// 模块预加载阶段，预加载到内存中的对象的列表的副本
        /// </summary>
        /// <remarks>
        /// key: 对象的检索路径
        /// object: 对象的实例
        /// </remarks>
        public Dictionary<string, object> preloadsRepetition { get; set; }
        


        public IHealthyViewBridge viewBridgeHealthy { get; set; }


        protected MyEntryBase entry_ { get; set; }
        protected LibMVCS.Logger logger_ { get; set; }
        protected MyConfig config_ { get; set; }
        protected MyCatalog catalog_ { get; set; }
        protected MyConfig.Style style_ { get; set; }
        protected Dictionary<string, LibMVCS.Any> settings_ { get; set; }
        protected MonoBehaviour mono_ {get;set;}

        public MyInstanceBase(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
        {
            uid = _uid;
            config_ = _config;
            catalog_ = _catalog;
            logger_ = _logger;
            settings_ = _settings;
            entry_ = _entry;
            mono_ = _mono;
            rootAttachments = _rootAttachments;
            foreach(var style in config_.styles)
            {
                if (style.name.Equals(_style))
                {
                    style_ = style;
                    break;
                }
            }
            contentObjectsPool = new ObjectsPool(uid + ".Content", logger_);
            themeObjectsPool = new ObjectsPool(uid + ".Theme", logger_);
        }

        /// <summary>
        /// 实例化UI
        /// </summary>
        /// <param name="_instanceUI">ui的实例模板</param>
        /// <param name="_parent">父对象</param>
        public void InstantiateUI(GameObject _instanceUI, Transform _parent)
        {
            rootUI = Object.Instantiate(_instanceUI, _parent);
            rootUI.name = uid;
        }
        
        /// <summary>
        /// 实例化World
        /// </summary>
        /// <param name="_instanceWorld">world的实例模板</param>
        /// <param name="_parent">父对象</param>
        public void InstantiateWorld(GameObject _instanceWorld, Transform _parent)
        {
            rootWorld = Object.Instantiate(_instanceWorld, _parent);
            rootWorld.name = uid;
        }

        public void SetupBridges()
        {

            var facadeHealthy = entry_.getDynamicHealthyFacade(uid);
            var bridgeHealthy = new HealthyUiBridge();
            bridgeHealthy.logger = logger_;
            facadeHealthy.setUiBridge(bridgeHealthy);
            viewBridgeHealthy = facadeHealthy.getViewBridge() as IHealthyViewBridge;

        }

        /// <summary>
        /// 当被显示时
        /// </summary>
        public virtual void HandleShowed()
        {
            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);
        }

        /// <summary>
        /// 当被隐藏时
        /// </summary>
        public virtual void HandleHided()
        {
            rootUI.gameObject.SetActive(false);
            rootWorld.gameObject.SetActive(false);
        }

        /// <summary>
        /// 使用消息订阅发布主题
        /// </summary>
        /// <param name="_subjects">主题列表</param>
        /// <param name="_variableS">变量字典</param>
        protected void publishSubjects(MyConfig.Subject[] _subjects, Dictionary<string, object> _variableS)
        {
            foreach (var subject in _subjects)
            {
                var data = new Dictionary<string, object>();
                foreach (var parameter in subject.parameters)
                {
                    if (_variableS.ContainsKey(parameter.value))
                    {
                        data[parameter.key] = _variableS[parameter.value];
                    }
                    else
                    {
                        if (parameter.type.Equals("string"))
                            data[parameter.key] = parameter.value;
                        else if (parameter.type.Equals("int"))
                            data[parameter.key] = int.Parse(parameter.value);
                        else if (parameter.type.Equals("float"))
                            data[parameter.key] = float.Parse(parameter.value);
                        else if (parameter.type.Equals("bool"))
                            data[parameter.key] = bool.Parse(parameter.value);
                    }
                }
                logger_.Trace("publish {0}", subject.message);
                entry_.getDummyModel().Publish(subject.message, data);
            }
        }

        /// <summary>
        /// 将目标按锚点在父对象中对齐
        /// </summary>
        /// <param name="_target">目标</param>
        /// <param name="_anchor">锚点</param>
        protected void alignByAncor(Transform _target, MyConfig.Anchor _anchor)
        {
            if (null == _target)
                return;
            RectTransform rectTransform = _target.GetComponent<RectTransform>();
            if (null == rectTransform)
                return;

            RectTransform parent = _target.transform.parent.GetComponent<RectTransform>();
            float marginH = 0;
            if (_anchor.marginH.EndsWith("%"))
            {
                float margin = 0;
                float.TryParse(_anchor.marginH.Replace("%", ""), out margin);
                marginH = (margin / 100.0f) * (parent.rect.width / 2);
            }
            else
            {
                float.TryParse(_anchor.marginH, out marginH);
            }

            float marginV = 0;
            if (_anchor.marginV.EndsWith("%"))
            {
                float margin = 0;
                float.TryParse(_anchor.marginV.Replace("%", ""), out margin);
                marginV = (margin / 100.0f) * (parent.rect.height / 2);
            }
            else
            {
                float.TryParse(_anchor.marginV, out marginV);
            }

            Vector2 anchorMin = new Vector2(0.5f, 0.5f);
            Vector2 anchorMax = new Vector2(0.5f, 0.5f);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            if (_anchor.horizontal.Equals("left"))
            {
                anchorMin.x = 0;
                anchorMax.x = 0;
                pivot.x = 0;
            }
            else if (_anchor.horizontal.Equals("right"))
            {
                anchorMin.x = 1;
                anchorMax.x = 1;
                pivot.x = 1;
                marginH *= -1;
            }

            if (_anchor.vertical.Equals("top"))
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
                pivot.y = 1;
                marginV *= -1;
            }
            else if (_anchor.vertical.Equals("bottom"))
            {
                anchorMin.y = 0;
                anchorMax.y = 0;
                pivot.y = 0;
            }

            Vector2 position = new Vector2(marginH, marginV);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(_anchor.width, _anchor.height);
        }


        /// <summary>
        /// 从主题目录中加载纹理
        /// </summary>
        /// <param name="_file">文件相对于 themes/{ModuleName} 的路径</param>
        protected void loadTextureFromTheme(string _file, System.Action<Texture2D> _onFinish, System.Action _onError)
        {
            string path = settings_["path.themes"].AsString();
            path = System.IO.Path.Combine(path, MyEntryBase.ModuleName);
            string filefullpath = System.IO.Path.Combine(path, _file);
            themeObjectsPool.LoadTexture(filefullpath, null, _onFinish, _onError);
        }

        /// <summary>
        /// 从主题目录中加载文本
        /// </summary>
        /// <param name="_file">文件相对于 themes/{ModuleName} 的路径</param>
        protected void loadTextFromTheme(string _file, System.Action<byte[]> _onFinish, System.Action _onError)
        {
            string path = settings_["path.themes"].AsString();
            path = System.IO.Path.Combine(path, MyEntryBase.ModuleName);
            string filefullpath = System.IO.Path.Combine(path, _file);
            themeObjectsPool.LoadText(filefullpath, null, _onFinish, _onError);
        }
        
        /// <summary>
        /// 从主题目录加载音频
        /// </summary>
        /// <param name="_file">文件相对于 themes/{ModuleName} 的路径</param>
        protected void loadAudioFromTheme(string _file, System.Action<AudioClip> _onFinish, System.Action _onError)
        {
            string path = settings_["path.themes"].AsString();
            path = System.IO.Path.Combine(path, MyEntryBase.ModuleName);
            string filefullpath = System.IO.Path.Combine(path, _file);
            themeObjectsPool.LoadAudioClip(filefullpath, null, _onFinish, _onError);
        }


        protected virtual void submitHealthyEcho(HealthyEchoRequest _request)
        {
            var dto = new HealthyEchoRequestDTO(_request);
            SynchronizationContext context = SynchronizationContext.Current;
            Task.Run(async () =>
            {
                try
                {
                    var reslut = await viewBridgeHealthy.OnEchoSubmit(dto, context);
                    if (!LibMVCS.Error.IsOK(reslut))
                    {
                        logger_.Error(reslut.getMessage());
                    }
                }
                catch (System.Exception ex)
                {
                    logger_.Exception(ex);
                }
            });
        }


    }
}
