using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace RetroSpy
{
    public class ElementConfig
    {
        public BitmapImage Image { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint OriginalX { get; set; }
        public uint OriginalY { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint OriginalWidth { get; set; }
        public uint OriginalHeight { get; set; }
        public List<string> TargetBackgrounds { get; private set; }
        public List<string> IgnoreBackgrounds { get; private set; }
        public void SetTargetBackgrounds(List<string> list)
        {
            if (TargetBackgrounds == null)
                TargetBackgrounds = list;
            else
            {
                TargetBackgrounds.Clear();
                TargetBackgrounds.AddRange(list);
            }
        }

        public void SetIgnoreBackgrounds(List<string> list)
        {
            if (IgnoreBackgrounds == null)
                IgnoreBackgrounds = list;
            else
            {
                IgnoreBackgrounds.Clear();
                IgnoreBackgrounds.AddRange(list);
            }
        }
    }

    public class Background
    {
        public string Name { get; set; }
        public BitmapImage Image { get; set; }
        public Color Color { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
    }

    public class Detail
    {
        public string Name { get; set; }
        public ElementConfig Config { get; set; }
    }

    public class Button
    {
        public ElementConfig Config { get; set; }
        public string Name { get; set; }
    }

    public class RangeButton
    {
        public ElementConfig Config { get; set; }
        public string Name { get; set; }
        public float From { get; set; }
        public float To { get; set; }
    }

    public class AnalogStick
    {
        public ElementConfig Config { get; set; }
        public string XName { get; set; }
        public string YName { get; set; }
        public string VisibilityName { get; set; }
        public uint XRange { get; set; }
        public uint YRange { get; set; }
        public uint OriginalXRange { get; set; }
        public uint OriginalYRange { get; set; }
        public bool XReverse { get; set; }
        public bool YReverse { get; set; }
    }

    public class AnalogTrigger
    {
        public enum DirectionValue { Up, Down, Left, Right, Fade }

        public ElementConfig Config { get; set; }
        public string Name { get; set; }
        public DirectionValue Direction { get; set; }
        public bool IsReversed { get; set; }
        public bool UseNegative { get; set; }
    }

    public class TouchPad
    {
        public ElementConfig Config { get; set; }
        public string XName { get; set; }
        public string YName { get; set; }
        public uint XRange { get; set; }
        public uint YRange { get; set; }
        public uint OriginalXRange { get; set; }
        public uint OriginalYRange { get; set; }
    }

    public class LoadResults
    {
        public List<Skin> SkinsLoaded { get; private set; }
        public List<string> ParseErrors { get; private set; }

        public void SetSkinsLoaded(List<Skin> list)
        {
            if (SkinsLoaded == null)
                SkinsLoaded = list;
            else
            {
                SkinsLoaded.Clear();
                SkinsLoaded.AddRange(list);
            }
        }

        public void SetParseErrors(List<string> list)
        {
            if (ParseErrors == null)
                ParseErrors = list;
            else
            {
                ParseErrors.Clear();
                ParseErrors.AddRange(list);
            }
        }
    }

    public class Skin
    {
        private readonly ResourceManager _resources;
        public string Name { get; private set; }
        public string Author { get; private set; }
        public InputSource Type { get; private set; }

        private readonly List<Background> _backgrounds = new List<Background>();
        public IReadOnlyList<Background> Backgrounds => _backgrounds;

        private readonly List<Detail> _details = new List<Detail>();
        public IReadOnlyList<Detail> Details => _details;

        private readonly List<Button> _buttons = new List<Button>();
        public IReadOnlyList<Button> Buttons => _buttons;

        private readonly List<RangeButton> _rangeButtons = new List<RangeButton>();
        public IReadOnlyList<RangeButton> RangeButtons => _rangeButtons;

        private readonly List<AnalogStick> _analogSticks = new List<AnalogStick>();
        public IReadOnlyList<AnalogStick> AnalogSticks => _analogSticks;

        private readonly List<AnalogTrigger> _analogTriggers = new List<AnalogTrigger>();
        public IReadOnlyList<AnalogTrigger> AnalogTriggers => _analogTriggers;

        private readonly List<TouchPad> _touchPads = new List<TouchPad>();
        public IReadOnlyList<TouchPad> TouchPads => _touchPads;

        // ----------------------------------------------------------------------------------------------------------------

        private Skin()
        { }

        private Skin(string folder, List<Skin> generatedSkins)
        {
            _resources = Properties.Resources.ResourceManager;
            string skinPath = Path.Combine(Environment.CurrentDirectory, folder);

            if (!File.Exists(Path.Combine(skinPath, "skin.xml")))
            {
                //throw new ConfigParseException ("Could not find skin.xml for skin at '"+folder+"'.");
                return;
            }
            XDocument doc = XDocument.Load(Path.Combine(skinPath, "skin.xml"));

            Name = ReadStringAttr(doc.Root, "name");
            Author = ReadStringAttr(doc.Root, "author");

            string typeStr = ReadStringAttr(doc.Root, ("type"));
            string[] typesVec = typeStr.Split(';');

            List<InputSource> types = new List<InputSource>();

            foreach (string type in typesVec)
            {
                InputSource TempType = InputSource.ALL.First(x => x.TypeTag == type);

                if (TempType == null)
                {
                    throw new ConfigParseException(_resources.GetString("IllegalSkinType", CultureInfo.CurrentUICulture));
                }
                types.Add(TempType);
            }

            int i = 0;
            foreach (InputSource inputSource in types)
            {
                Skin TempSkin = null;
                if (i == 0)
                {
                    TempSkin = this;
                    i++;
                }
                else
                {
                    TempSkin = new Skin();
                }

                TempSkin.LoadSkin(Name, Author, inputSource, doc, skinPath);
                generatedSkins.Add(TempSkin);
            }
        }

        public void LoadSkin(string name, string author, InputSource type, XDocument doc, string skinPath)
        {
            Name = name;
            Author = author;
            Type = type;

            if (doc == null)
                throw new NullReferenceException();

            IEnumerable<XElement> bgElems = doc.Root.Elements("background");

            if (!bgElems.Any())
            {
                throw new ConfigParseException(_resources.GetString("OneBackground", CultureInfo.CurrentUICulture));
            }

            foreach (XElement elem in bgElems)
            {
                string imgPath = ReadStringAttr(elem, "image", false);
                BitmapImage image = null;
                uint width = 0;
                uint height = 0;
                if (!string.IsNullOrEmpty(imgPath))
                {
                    image = LoadImage(skinPath, imgPath);
                    width = (uint)image.PixelWidth;
                    IEnumerable<XAttribute> widthAttr = elem.Attributes("width");
                    if (widthAttr.Any())
                    {
                        width = uint.Parse(widthAttr.First().Value, CultureInfo.CurrentCulture);
                    }

                    height = (uint)image.PixelHeight;
                    IEnumerable<XAttribute> heightAttr = elem.Attributes("height");
                    if (heightAttr.Any())
                    {
                        height = uint.Parse(heightAttr.First().Value, CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    IEnumerable<XAttribute> widthAttr = elem.Attributes("width");
                    if (widthAttr.Any())
                    {
                        width = uint.Parse(widthAttr.First().Value, CultureInfo.CurrentCulture);
                    }

                    IEnumerable<XAttribute> heightAttr = elem.Attributes("height");
                    if (heightAttr.Any())
                    {
                        height = uint.Parse(heightAttr.First().Value, CultureInfo.CurrentCulture);
                    }

                    if (width == 0 || height == 0)
                    {
                        throw new ConfigParseException(_resources.GetString("BothWidthAndHeight", CultureInfo.CurrentUICulture));
                    }
                }
                _backgrounds.Add(new Background
                {
                    Name = ReadStringAttr(elem, "name"),
                    Image = image,
                    Color = ReadColorAttr(elem, "color", false),
                    Width = width,
                    Height = height
                });
            }

            foreach (XElement elem in doc.Root.Elements("detail"))
            {
                _details.Add(new Detail
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name"),
                });
            }

            foreach (XElement elem in doc.Root.Elements("button"))
            {
                _buttons.Add(new Button
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name")
                });
            }

            foreach (XElement elem in doc.Root.Elements("rangebutton"))
            {
                float from = ReadFloatConfig(elem, "from");
                float to = ReadFloatConfig(elem, "to");

                if (from > to)
                {
                    throw new ConfigParseException(_resources.GetString("FromCannotBeGreaterThanTo", CultureInfo.CurrentUICulture));
                }

                _rangeButtons.Add(new RangeButton
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name"),
                    From = from,
                    To = to
                });
            }

            foreach (XElement elem in doc.Root.Elements("stick"))
            {
                _analogSticks.Add(new AnalogStick
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    XName = ReadStringAttr(elem, "xname"),
                    YName = ReadStringAttr(elem, "yname"),
                    VisibilityName = ReadStringAttr(elem, "visname", false),
                    XRange = ReadUintAttr(elem, "xrange"),
                    OriginalXRange = ReadUintAttr(elem, "xrange"),
                    YRange = ReadUintAttr(elem, "yrange"),
                    OriginalYRange = ReadUintAttr(elem, "yrange"),
                    XReverse = ReadBoolAttr(elem, "xreverse"),
                    YReverse = ReadBoolAttr(elem, "yreverse")
                });
            }

            foreach (XElement elem in doc.Root.Elements("touchpad"))
            {
                _touchPads.Add(new TouchPad
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    XName = ReadStringAttr(elem, "xname"),
                    YName = ReadStringAttr(elem, "yname"),
                    XRange = ReadUintAttr(elem, "xrange"),
                    OriginalXRange = ReadUintAttr(elem, "xrange"),
                    YRange = ReadUintAttr(elem, "yrange"),
                    OriginalYRange = ReadUintAttr(elem, "yrange"),
                });
            }

            foreach (XElement elem in doc.Root.Elements("analog"))
            {
                IEnumerable<XAttribute> directionAttrs = elem.Attributes("direction");
                if (!directionAttrs.Any())
                {
                    throw new ConfigParseException(_resources.GetString("AnalogNeedsDirection", CultureInfo.CurrentUICulture));
                }

                AnalogTrigger.DirectionValue dir;

                switch (directionAttrs.First().Value)
                {
                    case "up": dir = AnalogTrigger.DirectionValue.Up; break;
                    case "down": dir = AnalogTrigger.DirectionValue.Down; break;
                    case "left": dir = AnalogTrigger.DirectionValue.Left; break;
                    case "right": dir = AnalogTrigger.DirectionValue.Right; break;
                    case "fade": dir = AnalogTrigger.DirectionValue.Fade; break;
                    default: throw new ConfigParseException(_resources.GetString("IllegalDirection", CultureInfo.CurrentUICulture));
                }

                _analogTriggers.Add(new AnalogTrigger
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name"),
                    Direction = dir,
                    IsReversed = ReadBoolAttr(elem, "reverse"),
                    UseNegative = ReadBoolAttr(elem, "usenegative")
                });
            }
        }

        private static string ReadStringAttr(XElement elem, string attrName, bool required = true)
        {
            IEnumerable<XAttribute> attrs = elem.Attributes(attrName);
            if (!attrs.Any())
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'.");
                }
                else
                {
                    return "";
                }
            }
            return attrs.First().Value;
        }

        private static List<string> GetArrayAttr(XElement elem, string attrName, bool required = true)
        {
            IEnumerable<XAttribute> attrs = elem.Attributes(attrName);
            if (!attrs.Any())
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'. You can use it with ';' for multiple values.");
                }
                else
                {
                    return new List<string>(0);
                }
            }
            return new List<string>(attrs.First().Value.Split(';'));
        }

        private static Color ReadColorAttr(XElement elem, string attrName, bool required)
        {
            Color result = new Color();

            IEnumerable<XAttribute> attrs = elem.Attributes(attrName);
            if (!attrs.Any())
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'.");
                }
                else
                {
                    return result;
                }
            }
            object converted = ColorConverter.ConvertFromString(attrs.First().Value);
            if (result != null)
            {
                result = (Color)converted;
            }
            return result;
        }

        private static float ReadFloatConfig(XElement elem, string attrName)
        {
            if (!float.TryParse(ReadStringAttr(elem, attrName), out float ret))
            {
                throw new ConfigParseException("Failed to parse number for property '" + attrName + "' in element '" + elem.Name + "'.");
            }
            return ret;
        }

        private static uint ReadUintAttr(XElement elem, string attrName)
        {
            if (!uint.TryParse(ReadStringAttr(elem, attrName), out uint ret))
            {
                throw new ConfigParseException("Failed to parse number for property '" + attrName + "' in element '" + elem.Name + "'.");
            }
            return ret;
        }

        private static BitmapImage LoadImage(string skinPath, string fileName)
        {
            try
            {
                return new BitmapImage(new Uri(Path.Combine(skinPath, fileName)));
            }
            catch (Exception e)
            {
                throw new ConfigParseException("Could not load image '" + fileName + "'.", e);
            }
        }

        private static ElementConfig ParseStandardConfig(string skinPath, XElement elem)
        {
            IEnumerable<XAttribute> imageAttr = elem.Attributes("image");
            if (!imageAttr.Any())
            {
                throw new ConfigParseException("Attribute 'image' missing for element '" + elem.Name + "'.");
            }

            BitmapImage image = LoadImage(skinPath, imageAttr.First().Value);

            uint width = (uint)image.PixelWidth;
            IEnumerable<XAttribute> widthAttr = elem.Attributes("width");
            if (widthAttr.Any())
            {
                width = uint.Parse(widthAttr.First().Value, CultureInfo.CurrentCulture);
            }

            uint height = (uint)image.PixelHeight;
            IEnumerable<XAttribute> heightAttr = elem.Attributes("height");
            if (heightAttr.Any())
            {
                height = uint.Parse(heightAttr.First().Value, CultureInfo.CurrentCulture);
            }

            uint x = ReadUintAttr(elem, "x");
            uint y = ReadUintAttr(elem, "y");

            List<string> targetBgs = GetArrayAttr(elem, "target", false);
            List<string> ignoreBgs = GetArrayAttr(elem, "ignore", false);

            var ec = new ElementConfig
            {
                X = x,
                Y = y,
                OriginalX = x,
                OriginalY = y,
                Image = image,
                Width = width,
                OriginalWidth = width,
                Height = height,
                OriginalHeight = height,
            };
            ec.SetTargetBackgrounds(targetBgs);
            ec.SetIgnoreBackgrounds(ignoreBgs);

            return ec;
        }

        private static bool ReadBoolAttr(XElement elem, string attrName, bool dfault = false)
        {
            IEnumerable<XAttribute> attrs = elem.Attributes(attrName);
            if (!attrs.Any())
            {
                return dfault;
            }

            if (attrs.First().Value == "true")
            {
                return true;
            }

            if (attrs.First().Value == "false")
            {
                return false;
            }

            return dfault;
        }

        // ----------------------------------------------------------------------------------------------------------------

        public static void LoadAllSkinsFromSubFolder(string path, List<Skin> skins, List<string> errs)
        {
            if (skins == null || errs == null)
                throw new NullReferenceException();

            foreach (string skinDir in Directory.GetDirectories(path))
            {
                try
                {
                    List<Skin> generatedSkins = new List<Skin>();
                    Skin skin;
                    try
                    {
                        skin = new Skin(skinDir, generatedSkins);
                    }
                    catch (ConfigParseException e)
                    {
                        errs.Add(skinDir + " :: " + e.Message);
                        continue;
                    }
                    foreach (Skin generatedSkin in generatedSkins)
                    {
                        skins.Add(generatedSkin);
                    }
                }
                catch (ConfigParseException e)
                {
                    errs.Add(skinDir + " :: " + e.Message);
                }
                LoadAllSkinsFromSubFolder(skinDir, skins, errs);
            }
        }

        public static LoadResults LoadAllSkinsFromParentFolder(string path)
        {
            List<Skin> skins = new List<Skin>();
            List<string> errs = new List<string>();

            LoadAllSkinsFromSubFolder(path, skins, errs);

            var lr = new LoadResults();
            lr.SetSkinsLoaded(skins);
            lr.SetParseErrors(errs);
            return lr;
        }
    }
}