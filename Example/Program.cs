using System;
using System.Diagnostics;
using emailBuilder;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var mailBuilder = new MailBuilder {new TextComponent("hi")};
            Debug.WriteLine(mailBuilder.Build());
        }
    }
}