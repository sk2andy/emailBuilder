# emailBuilder

this is a small project to create email html in C# .NET (core)

## Components

you can use these components or build your own ones by deriving from `IMailComponent`
* TitleComponent
* TextComponent
* RowComponent
* ColumnComponent
* SeparatorComponent
* ImageComponent
* ButtonComponent

## How to use

Clone git repo or add [Nuget](https://www.nuget.org/packages/EmailBuilder/1.0.0) package

start with using the `MailBuilder` like this:

```
var mailBuilder = new MailBuilder
{
    new RowComponent {new TitleComponent("Your Title", new Uri("your title image"))},
    new RowComponent
    {
        new ColumnComponent(4) {new TextComponent("some text")},
        new ColumnComponent(8) {new TextComponent("some other text")}
    }
}
```

and when you added all your stuff simply run `mailbuilder.Build()` to get the html content string for your email
