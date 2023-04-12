
using System.Xml.Serialization;

namespace XTC.FMP.MOD.MediaCarousel.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class Slide
        {
            [XmlAttribute("type")]
            public string type { get; set; } = "";
            [XmlArray("OnCreatedSubjectS"), XmlArrayItem("Subject")]
            public Subject[] onCreatedSubjectS { get; set; } = new Subject[0];
            [XmlArray("OnActivatedSubjectS"), XmlArrayItem("Subject")]
            public Subject[] onActivatedSubjectS { get; set; } = new Subject[0];
        }

        public class ClickArea
        {

            [XmlArray("OnClickSubjectS"), XmlArrayItem("Subject")]
            public Subject[] onClickSubjectS { get; set; } = new Subject[0];
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlAttribute("autoSwitch")]
            public bool autoSwitch { get; set; } = true;
            [XmlElement("ClickArea")]
            public ClickArea clickArea { get; set; }
            [XmlElement("ButtonPrev")]
            public UiElement btnPrev { get; set; } = new UiElement();
            [XmlElement("ButtonNext")]
            public UiElement btnNext { get; set; } = new UiElement();
            [XmlArray("SlideS"), XmlArrayItem("Slide")]
            public Slide[] slideS { get; set; } = new Slide[0];
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

