
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.83.0.  DO NOT EDIT!
//*************************************************************************************

namespace XTC.FMP.MOD.MediaCarousel.LIB.Unity
{
    public class MySubjectBase
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["style"] = "default";
        /// data["uiSlot"] = "";
        /// data["worldSlot"] = "";
        /// model.Publish(/XTC/MediaCarousel/Create, data);
        /// </example>
        public const string Create = "/XTC/MediaCarousel/Create";

        /// <summary>
        /// 打开
        /// </summary>
        /// <remarks>
        /// 先加载资源，然后显示
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["source"] = "file";
        /// data["uri"] = "";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/MediaCarousel/Open, data);
        /// </example>
        public const string Open = "/XTC/MediaCarousel/Open";

        /// <summary>
        /// 显示
        /// </summary>
        /// <remarks>
        /// 仅显示，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/MediaCarousel/Show, data);
        /// </example>
        public const string Show = "/XTC/MediaCarousel/Show";

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <remarks>
        /// 仅隐藏，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/MediaCarousel/Hide, data);
        /// </example>
        public const string Hide = "/XTC/MediaCarousel/Hide";

        /// <summary>
        /// 关闭
        /// </summary>
        /// <remarks>
        /// 先隐藏，然后释放资源
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/MediaCarousel/Close, data);
        /// </example>
        public const string Close = "/XTC/MediaCarousel/Close";

        /// <summary>
        /// 销毁
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/MediaCarousel/Close, data);
        /// </example>
        public const string Delete = "/XTC/MediaCarousel/Delete";
    }
}