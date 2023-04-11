

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
            public int index;
            public string source;
            public int duration;
        }

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


            int slideIndex = 0;
            foreach (var section in catalog_.sectionS)
            {
                foreach (var content in section.contentS)
                {
                    Slide slide = new Slide();
                    slide.index = slideIndex;
                    slideIndex += 1;
                    slideS_.Add(slide);
                    contentReader_.ContentUri = content;
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
                                slide.source = string.Format("{0}/_attachments/{1}", schema.foreign_bundle_uuid, file.path);
                                logger_.Info(slide.source);
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
                        if (valueSlide == "Image")
                        {
                            Dictionary<string, object> openedVariableS = new Dictionary<string, object>();
                            openedVariableS["{{slide_uuid}}"] = string.Format("XTC_MediaCarousel_{0}_slide_{1}", uid, slide.index);
                            publishSubjects(style_.imageSlide.onOpenedSubjectS, openedVariableS);
                        }
                    }, () => { });
                }
            }

            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);
            coroutineTick_ = mono_.StartCoroutine(tick());
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
                timer_ += 1;
                var slide = slideS_[currentSlideIndex_];
                if (timer_ > slide.duration)
                {
                    currentSlideIndex_ += 1;
                    if (currentSlideIndex_ >= slideS_.Count)
                        currentSlideIndex_ = 0;
                    timer_ = 0;
                    //publishSubjects();
                }
            }
        }
    }
}
