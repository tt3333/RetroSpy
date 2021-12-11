using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace RetroSpy
{
    [CLSCompliant(false)]
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
        public Collection<string> TargetBackgrounds { get; private set; }
        public Collection<string> IgnoreBackgrounds { get; private set; }
        public void SetTargetBackgrounds(Collection<string> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (TargetBackgrounds == null)
                TargetBackgrounds = list;
            else
            {
                TargetBackgrounds.Clear();
                foreach (var background in list)
                    TargetBackgrounds.Add(background);
            }
        }

        public void SetIgnoreBackgrounds(Collection<string> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (IgnoreBackgrounds == null)
                IgnoreBackgrounds = list;
            else
            {
                IgnoreBackgrounds.Clear();
                foreach (var background in list)
                    IgnoreBackgrounds.Add(background);
            }
        }
    }

    [CLSCompliant(false)]
    public class Background
    {
        public string Name { get; set; }
        public BitmapImage Image { get; set; }
        public Color Color { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
    }

    [CLSCompliant(false)]
    public class Detail
    {
        public string Name { get; set; }
        public ElementConfig Config { get; set; }
    }

    [CLSCompliant(false)]
    public class Button
    {
        public ElementConfig Config { get; set; }
        public string Name { get; set; }
        public float Precision { get; set; }
    }

    [CLSCompliant(false)]
    public class RangeButton
    {
        public ElementConfig Config { get; set; }
        public string Name { get; set; }
        public float From { get; set; }
        public float To { get; set; }
    }

    [CLSCompliant(false)]
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
        public float XPrecision { get; set; }
        public float YPrecision { get; set; }
    }

    [CLSCompliant(false)]
    public class AnalogTrigger
    {
        public enum DirectionValue { Up, Down, Left, Right, Fade }

        public ElementConfig Config { get; set; }
        public string Name { get; set; }
        public DirectionValue Direction { get; set; }
        public bool IsReversed { get; set; }
        public bool UseNegative { get; set; }
    }

    [CLSCompliant(false)]
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

    [CLSCompliant(false)]
    public class AnalogText
    {

        public uint X { get; set; }
        public uint Y { get; set; }
        public uint OriginalX { get; set; }
        public uint OriginalY { get; set; }
        public FontFamily Font { get; set; }
        public Brush Color { get; set; }
        public uint Range { get; set; }
        public float Size { get; set; }
        public string Name { get; set; }
        public Collection<string> TargetBackgrounds { get; private set; }
        public Collection<string> IgnoreBackgrounds { get; private set; }
        public void SetTargetBackgrounds(Collection<string> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (TargetBackgrounds == null)
                TargetBackgrounds = list;
            else
            {
                TargetBackgrounds.Clear();
                foreach (var background in list)
                    TargetBackgrounds.Add(background);
            }
        }

        public void SetIgnoreBackgrounds(Collection<string> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (IgnoreBackgrounds == null)
                IgnoreBackgrounds = list;
            else
            {
                IgnoreBackgrounds.Clear();
                foreach (var background in list)
                    IgnoreBackgrounds.Add(background);
            }
        }
    }

    public class LoadResults
    {
        [CLSCompliant(false)]
        public Collection<Skin> SkinsLoaded { get; private set; }
        public Collection<string> ParseErrors { get; private set; }

        [CLSCompliant(false)]
        public void SetSkinsLoaded(Collection<Skin> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (SkinsLoaded == null)
                SkinsLoaded = list;
            else
            {
                SkinsLoaded.Clear();
                foreach (var skin in list)
                    SkinsLoaded.Add(skin);
            }
        }

        public void SetParseErrors(Collection<string> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (ParseErrors == null)
                ParseErrors = list;
            else
            {
                ParseErrors.Clear();
                foreach (var parseError in list)
                    ParseErrors.Add(parseError);
            }
        }
    }

    [CLSCompliant(false)]
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

        private readonly List<AnalogText> _analogTexts = new List<AnalogText>();
        public IReadOnlyList<AnalogText> AnalogTexts => _analogTexts;

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

            List<Tuple<InputSource, string>> types = new List<Tuple<InputSource, string>>();

            foreach (string type in typesVec)
            {
                string orgType = type;
                string type1;
                if (type.Contains("."))
                {
                    type1 = type.Substring(0, type.IndexOf('.'));
                }
                else
                {
                    type1 = type;
                }
                types.Add(new Tuple<InputSource, string>(InputSource.ALL.First(x => x.TypeTag == type1), orgType));

            }

            int i = 0;
            foreach (Tuple<InputSource, string> inputSource in types)
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

                TempSkin.LoadSkin(Name, Author, inputSource.Item1, doc, skinPath, inputSource.Item2);
                generatedSkins.Add(TempSkin);
            }
        }

        public void LoadSkin(string name, string author, InputSource type, XDocument doc, string skinPath, string orgType)
        {
            Name = name;
            Author = author;
            Type = type;

            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

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

            ParseElements(doc.Root, skinPath);

            foreach (XElement elem in doc.Root.Elements("section"))
            {
                var sectionType = ReadStringAttr(elem, "type");

                var sectionTypes = sectionType.Split(';');

                if (sectionTypes.Contains(orgType))
                {
                    name = ReadStringAttr(elem, "name", false);
                    if (!string.IsNullOrEmpty(name))
                        Name = name;

                    ParseElements(elem, skinPath);
                }
            }
        }

        private void ParseElements(XElement doc, string skinPath)
        {
            foreach (XElement elem in doc.Elements("detail"))
            {
                _details.Add(new Detail
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name"),
                });
            }

            foreach (XElement elem in doc.Elements("button"))
            {
                _buttons.Add(new Button
                {
                    Config = ParseStandardConfig(skinPath, elem),
                    Name = ReadStringAttr(elem, "name"),
                    Precision = ReadFloatConfig(elem, "precision", false)
                });
            }

            foreach (XElement elem in doc.Elements("rangebutton"))
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

            foreach (XElement elem in doc.Elements("stick"))
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
                    YReverse = ReadBoolAttr(elem, "yreverse"),
                    XPrecision = ReadFloatConfig(elem, "xprecision", false),
                    YPrecision = ReadFloatConfig(elem, "yprecision", false)
                });
            }

            foreach (XElement elem in doc.Elements("touchpad"))
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

            foreach (XElement elem in doc.Elements("analog"))
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

            foreach (XElement elem in doc.Elements("analogtext"))
            {
                IEnumerable<XAttribute> fontAttrs = elem.Attributes("font");
                if (!fontAttrs.Any())
                {
                    throw new ConfigParseException(_resources.GetString("AnalogTextNeedsFont", CultureInfo.CurrentUICulture));
                }
                FontFamily font = new FontFamily(fontAttrs.First().Value);

                IEnumerable<XAttribute> colorAttrs = elem.Attributes("color");
                if (!colorAttrs.Any())
                {
                    throw new ConfigParseException(_resources.GetString("AnalogTextNeedsColor", CultureInfo.CurrentUICulture));
                }

                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString(colorAttrs.First().Value);

                Collection<string> targetBgs = GetArrayAttr(elem, "target", false);
                Collection<string> ignoreBgs = GetArrayAttr(elem, "ignore", false);

                var x = ReadUintAttr(elem, "x");
                var y = ReadUintAttr(elem, "y");

                var analogTextConfig = new AnalogText
                {
                    X = x,
                    Y = y,
                    OriginalX = x,
                    OriginalY = y,
                    Size = ReadFloatConfig(elem, "size"),
                    Range = ReadUintAttr(elem, "range"),
                    Font = font,
                    Color = brush,
                    Name = ReadStringAttr(elem, "name"),
                };
                analogTextConfig.SetTargetBackgrounds(targetBgs);
                analogTextConfig.SetIgnoreBackgrounds(ignoreBgs);

                _analogTexts.Add(analogTextConfig);
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

        private static Collection<string> GetArrayAttr(XElement elem, string attrName, bool required = true)
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
                    return new Collection<string>();
                }
            }
            return new Collection<string>(attrs.First().Value.Split(';'));
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
            return (Color)ColorConverter.ConvertFromString(attrs.First().Value);
        }

        private static float ReadFloatConfig(XElement elem, string attrName, bool required = true)
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
                    return 0.0f;
                }
            }

            if (!float.TryParse(ReadStringAttr(elem, attrName), NumberStyles.Any, CultureInfo.InvariantCulture, out float ret))
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

            Collection<string> targetBgs = GetArrayAttr(elem, "target", false);
            Collection<string> ignoreBgs = GetArrayAttr(elem, "ignore", false);

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

        public static void LoadAllSkinsFromSubFolder(string path, Collection<Skin> skins, Collection<string> errs)
        {
            if (skins == null)
                throw new ArgumentNullException(nameof(skins));
            else if (errs == null)
                throw new ArgumentNullException(nameof(errs));

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

        public static LoadResults LoadAllSkinsFromParentFolder(string path, string customPath)
        {
            Collection<Skin> skins = new Collection<Skin>();
            Collection<string> errs = new Collection<string>();

            LoadAllSkinsFromSubFolder(path, skins, errs);
            if (!string.IsNullOrEmpty(customPath))
                LoadAllSkinsFromSubFolder(customPath, skins, errs);

            var lr = new LoadResults();
            lr.SetSkinsLoaded(skins);
            lr.SetParseErrors(errs);
            return lr;
        }
    }
}