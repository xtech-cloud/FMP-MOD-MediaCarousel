

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.MediaCarousel.LIB.Proto;
using XTC.FMP.MOD.MediaCarousel.LIB.MVCS;
using Newtonsoft.Json;
using System.Text;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;

namespace XTC.FMP.MOD.MediaCarousel.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        private int currentSlideIndex_ = 0;
        private ContentReader contentReader_ = null;

        public class Slide
        {
            public GameObject slot;
            public int index;
            public string contentUri;
            public string attachmentUri;
            public int duration;
            public string type;
        }

        public class UiRefrence
        {
            public Transform slideSlot;
        }

        private UiRefrence uiReference = new UiRefrence();

        private List<Slide> slideS_ = new List<Slide>();
        private float timer_ = 0;

        private Coroutine coroutineTick_ = null;

        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <remarks>
        /// 可用于加载主题目录的数据
        /// </remarks>
        public void HandleCreated()
        {
            if (null == style_)
            {
                logger_.Error("style not found");
                return;
            }

            var btnClickArea = rootUI.transform.Find("ClickArea").GetComponent<Button>();
            btnClickArea.gameObject.SetActive(false);
            if (null != style_.clickArea)
            {
                btnClickArea.gameObject.SetActive(true);
                btnClickArea.onClick.AddListener(() =>
                {
                    Dictionary<string, object> variableS = new Dictionary<string, object>();
                    publishSubjects(style_.clickArea.onClickSubjectS, variableS);
                });
            }

            uiReference.slideSlot = rootUI.transform.Find("SlideS/slot");
            uiReference.slideSlot.gameObject.SetActive(false);
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        /// <remarks>
        /// 可用于加载内容目录的数据
        /// </remarks>
        public void HandleOpened(string _source, string _uri)
        {
            contentReader_ = new ContentReader(contentObjectsPool);
            contentReader_.AssetRootPath = settings_["path.assets"].AsString();

            CounterSequence counterSequence = new CounterSequence(0);
            counterSequence.OnFinish = () =>
            {
                onSlideActivated();
                coroutineTick_ = mono_.StartCoroutine(tick());
            };

            int slideIndex = 0;
            foreach (var section in catalog_.sectionS)
            {
                foreach (var content in section.contentS)
                {
                    Slide slide = new Slide();
                    slide.slot = GameObject.Instantiate(uiReference.slideSlot.gameObject, uiReference.slideSlot.parent);
                    slide.slot.name = slideIndex.ToString();
                    slide.index = slideIndex;
                    slide.contentUri = content;
                    slideIndex += 1;
                    slideS_.Add(slide);
                    counterSequence.Dial();
                }
            }

            foreach (var slide in slideS_)
            {
                contentReader_.ContentUri = slide.contentUri;
                contentReader_.LoadText("meta.json", (_byte) =>
                {
                    string valueSlide = "";
                    int valueDuration = 0;
                    try
                    {
                        string json = Encoding.UTF8.GetString(_byte);
                        ContentMetaSchema schema = JsonConvert.DeserializeObject<ContentMetaSchema>(json);
                        foreach (var file in schema.AttachmentS)
                        {
                            slide.attachmentUri = string.Format("{0}/_attachments/{1}", schema.foreign_bundle_uuid, file.path);
                        }
                        string strDuration;
                        if (schema.kvS.TryGetValue("XTC_MediaCarousel/duration", out strDuration))
                        {
                            valueDuration = int.Parse(strDuration);
                        }
                        schema.kvS.TryGetValue("XTC_MediaCarousel/slide", out valueSlide);
                    }
                    catch (System.Exception ex)
                    {
                        logger_.Exception(ex);
                    }
                    slide.duration = valueDuration;
                    slide.type = valueSlide;
                    onSlideCreated(slide);
                    counterSequence.Tick();
                }, () => { });

            }

            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            rootUI.gameObject.SetActive(false);
            rootWorld.gameObject.SetActive(false);
            slideS_.Clear();
            contentReader_ = null;
            if (null != coroutineTick_)
            {
                mono_.StopCoroutine(coroutineTick_);
                coroutineTick_ = null;
            }
        }

        private IEnumerator tick()
        {
            currentSlideIndex_ = 0;
            timer_ = 0;
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (currentSlideIndex_ >= slideS_.Count)
                    continue;
                var slide = slideS_[currentSlideIndex_];
                timer_ += 1;
                if (timer_ > slide.duration)
                {
                    currentSlideIndex_ += 1;
                    if (currentSlideIndex_ >= slideS_.Count)
                        currentSlideIndex_ = 0;
                    timer_ = 0;
                    onSlideActivated();
                }
            }
        }

        private void onSlideActivated()
        {
            int oldIndex = currentSlideIndex_ - 1 < 0 ? slideS_.Count - 1 : currentSlideIndex_ - 1;
            if (currentSlideIndex_ >= slideS_.Count)
            {
                logger_.Error("want to active a slide out of range");
                return;
            }
            slideS_[oldIndex].slot.gameObject.SetActive(false);
            var slide = slideS_[currentSlideIndex_];
            slide.slot.gameObject.SetActive(true);
            Dictionary<string, object> openedVariableS = new Dictionary<string, object>();
            openedVariableS["{{slide_uuid}}"] = string.Format("XTC_MediaCarousel_{0}_slide_{1}", uid, slide.index);
            openedVariableS["{{slide_slot}}"] = getPathOfTransform(slide.slot.transform);
            openedVariableS["{{slide_uri}}"] = slide.attachmentUri;
            MyConfig.Slide foundSlide = null;
            foreach (var theSlide in style_.slideS)
            {
                if (slide.type == theSlide.type)
                    foundSlide = theSlide;
            }
            if (null == foundSlide)
            {
                logger_.Error("slide:{0} not found", slide.type);
                return;
            }
            publishSubjects(foundSlide.onActivatedSubjectS, openedVariableS);
        }

        private void onSlideCreated(Slide _slide)
        {
            MyConfig.Slide foundSlide = null;
            foreach (var slide in style_.slideS)
            {
                if (slide.type == _slide.type)
                    foundSlide = slide;
            }
            if (null == foundSlide)
            {
                logger_.Error("slide:{0} not found", _slide.type);
                return;
            }
            Dictionary<string, object> openedVariableS = new Dictionary<string, object>();
            openedVariableS["{{slide_uuid}}"] = string.Format("XTC_MediaCarousel_{0}_slide_{1}", uid, _slide.index);
            openedVariableS["{{slide_slot}}"] = getPathOfTransform(_slide.slot.transform);
            openedVariableS["{{slide_uri}}"] = _slide.attachmentUri;
            publishSubjects(foundSlide.onCreatedSubjectS, openedVariableS);
        }

        private string getPathOfTransform(Transform _transform)
        {
            string path = "";
            Transform parent = _transform;
            while (null != parent)
            {
                path = string.Format("/{0}", parent.name) + path;
                parent = parent.parent;
            }
            return path;
        }
    }
}
