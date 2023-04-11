
using System.Xml.Serialization;

namespace XTC.FMP.MOD.MediaCarousel.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class ImageSlide
        {
            [XmlElement("OnOpenedSubjectS")]
            public Subject[] onOpenedSubjectS { get; set; } = new Subject[0];
        }

        public class ClickArea
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = true;

            [XmlArray("OnClickSubjectS"), XmlArrayItem("Subject")]
            public Subject[] onClickSubjectS { get; set; } = new Subject[0];
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlElement("ClickArea")]
            public ClickArea clickArea { get; set; }
            [XmlElement("ImageSlide")]
            public ImageSlide imageSlide { get; set; } = new ImageSlide();
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

