using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace emailBuilder
{
    /// <summary>
    /// Interface to implement mail component
    /// </summary>
    public interface IMailComponent
    {
        /// <summary>
        /// Builds html of component
        /// </summary>
        /// <returns>built html of component</returns>
        string Build();
    }

    /// <inheritdoc cref="IMailComponent" />
    /// <summary>
    /// Component that is a container for other components
    /// </summary>
    public abstract class AbstractSuperComponent : List<IMailComponent>, IMailComponent
    {
        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            var assembly = typeof(MailBuilder).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream($"emailBuilder.Assets.Components.{FileName()}");
            
            var reader = new StreamReader(resource);
            var fileContents = reader.ReadToEnd();
            foreach (var component in this)
            {
                builder.Append(component.Build());
            }

            return AfterBuild(fileContents.Replace("{content}", builder.ToString()));
        }

        /// <summary>
        /// Get file name of component where html is stored
        /// </summary>
        /// <returns>file name of html</returns>
        protected abstract string FileName();

        /// <summary>
        /// Life cycle hook to modify content after built.
        /// This can be used to replace placeholders
        /// </summary>
        /// <param name="content">html content of component</param>
        /// <returns>html content of built component</returns>
        protected virtual string AfterBuild(string content)
        {
            return content;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Simple component that doesn't contain any other component
    /// </summary>
    public abstract class AbstractSimpleComponent : IMailComponent
    {
        private readonly string _content;

        /// <inheritdoc cref="IMailComponent" />
        protected AbstractSimpleComponent(string content)
        {
            _content = content;
        }

        /// <inheritdoc />
        public string Build()
        {
            var assembly = typeof(MailBuilder).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream($"emailBuilder.Assets.Components.{FileName()}");
            
            var reader = new StreamReader(resource);
            var fileContents = reader.ReadToEnd();
            return AfterBuild(fileContents.Replace("{content}", _content));
        }

        /// <summary>
        /// Get file name of component where html is stored
        /// </summary>
        /// <returns>file name of html</returns>
        protected abstract string FileName();

        /// <summary>
        /// Life cycle hook to modify content after built.
        /// This can be used to replace placeholders
        /// </summary>
        /// <param name="content">html content of component</param>
        /// <returns>html content of built component</returns>
        protected virtual string AfterBuild(string content)
        {
            return content;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Component for an email title
    /// </summary>
    public class TitleComponent : AbstractSimpleComponent
    {
        private readonly string _brandName;
        private readonly Uri _logoUri;
        private readonly string _separatorColor;
        private readonly string _backgroundColor;
        private readonly string _brandFontColor;
        private readonly string _title;

        /// <summary>
        /// component that represents the title of an email 
        /// </summary>
        /// <param name="brandName">name of your brand</param>
        /// <param name="title">title of an email</param>
        /// <param name="logoUri">uri of the logo that should be used</param>
        /// <param name="separatorColor">color of the separator that separates the title of the content</param>
        /// <param name="backgroundColor">background color of title</param>
        /// <param name="brandFontColor">font color of brand name</param>
        public TitleComponent(string brandName, string title, Uri logoUri, string separatorColor = "black", string backgroundColor = "transparent", string brandFontColor = "black") : base(title)
        {
            _brandName = brandName;
            _logoUri = logoUri;
            _separatorColor = separatorColor;
            _backgroundColor = backgroundColor;
            _brandFontColor = brandFontColor;
            _title = title;
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "title.html";
        }

        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content
                .Replace("{logo}", _logoUri.ToString())
                .Replace("{brand}", _brandName)
                .Replace("{backgroundColor}", _backgroundColor)
                .Replace("{separatorColor}", _separatorColor)
                .Replace("{brandFontColor}", _brandFontColor);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Text component for an email.
    /// You can modify Align, Style and PaddingBottom
    /// </summary>
    public class TextComponent : AbstractSimpleComponent
    {
        /// <summary>
        /// Alignment of text (left, center, right)
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// Style of text (css)
        /// </summary>
        public string Style { get; set; } = "font-size: 14px; line-height: 16px";

        /// <summary>
        /// padding-bottom of text
        /// </summary>
        public string PaddingBottom { get; set; } = "10px";

        /// <inheritdoc />
        /// <summary>
        /// constructor of text component
        /// </summary>
        /// <param name="text">text for component</param>
        /// <param name="align">align of the text</param>
        public TextComponent(string text, string align = "left") : base(text)
        {
            Align = align;
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "text_left.html";
        }

        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content.Replace("{align}", Align).Replace("{style}", Style)
                .Replace("{padding_bottom}", PaddingBottom);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Component to add a row to an email
    /// </summary>
    public class RowComponent : AbstractSuperComponent
    {
        /// <inheritdoc />
        protected override string FileName()
        {
            return "row.html";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Component to add a col to an email. This should be used together with row component
    /// </summary>
    public class ColumnComponent : AbstractSuperComponent
    {
        /// <summary>
        /// bootstrap width of col (2-12)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// constructor that takes col width (2-12)
        /// </summary>
        /// <param name="width">bootstrap width (2-12)</param>
        public ColumnComponent(int width)
        {
            Width = width;
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "col.html";
        }

        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content.Replace("{width}", Width.ToString());
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Seperator to seperate contents
    /// </summary>
    public class SeparatorComponent : AbstractSimpleComponent
    {
        /// <inheritdoc />
        public SeparatorComponent() : base("")
        {
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "seperator.html";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Component to add an image to an email
    /// </summary>
    public class ImageComponent : AbstractSimpleComponent
    {
        /// <summary>
        /// width of image as css
        /// </summary>
        public string Width { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Constructor that takes url and width of image
        /// </summary>
        /// <param name="url">url of image</param>
        /// <param name="width">width as css of image</param>
        public ImageComponent(string url, string width) : base(url)
        {
            Width = width;
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "image.html";
        }

        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content.Replace("{width}", Width);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Component to add a button to an email
    /// </summary>
    public class ButtonComponent : AbstractSimpleComponent
    {
        /// <summary>
        /// Target of button
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Alignment of button
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// Color of button
        /// </summary>
        public string Color { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Constructor that takes button's text, target and alignment
        /// </summary>
        /// <param name="text">text of button</param>
        /// <param name="target">target of button</param>
        /// <param name="align">alignment of button</param>
        /// <param name="color">color of the button</param>
        public ButtonComponent(string text, string target, string align = "left", string color = "blue") : base(text)
        {
            Target = target;
            Align = align;
            Color = color;
        }

        /// <inheritdoc />
        protected override string FileName()
        {
            return "button.html";
        }

        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content.Replace("{target}", Target).Replace("{align}", Align).Replace("{color}", Color);
        }
    }

    public class FooterComponent : AbstractSimpleComponent
    {
        private readonly string _entrySentence;
        private readonly string _companyName;
        private readonly string _phone;
        private readonly string _email;
        private readonly string _street;
        private readonly string _zipCode;
        private readonly string _city;

        public FooterComponent(string entrySentence, string companyName, string phone, string email, string street, string zipCode, string city) : base(companyName)
        {
            _entrySentence = entrySentence;
            _companyName = companyName;
            _phone = phone;
            _email = email;
            _street = street;
            _zipCode = zipCode;
            _city = city;
        }

        protected override string FileName()
        {
            return "footer.html";
        }
        
        /// <inheritdoc />
        protected override string AfterBuild(string content)
        {
            return content
                .Replace("{entrySentence}", _entrySentence)
                .Replace("{companyName}", _companyName)
                .Replace("{phone}", _phone)
                .Replace("{email}", _email)
                .Replace("{street}", _street)
                .Replace("{zipCode}", _zipCode)
                .Replace("{city}", _city);
        }
    }

    /// <inheritdoc cref="IMailComponent" />
    /// <summary>
    /// Mailbulder provides the posiblity to build an html mail with predefined components
    /// </summary>
    public class MailBuilder : List<IMailComponent>, IMailComponent
    {
        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            var assembly = typeof(MailBuilder).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("emailBuilder.Assets.Components.structure.html");
            
            var reader = new StreamReader(resource);
            var fileContents = reader.ReadToEnd();
            foreach (var mailComponent in this)
            {
                builder.Append(mailComponent.Build());
            }

            return fileContents.Replace("{content}", builder.ToString());
        }
    }
}