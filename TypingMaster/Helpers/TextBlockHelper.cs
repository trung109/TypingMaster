using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TypingMaster.Helpers
{
    public static class TextBlockHelper
    {
        public static IEnumerable<Inline> GetBindableInlines(DependencyObject obj)
        {
            return (IEnumerable<Inline>)obj.GetValue(BindableInlinesProperty);
        }

        public static void SetBindableInlines(DependencyObject obj,  IEnumerable<Inline> value)
        {
            obj.SetValue(BindableInlinesProperty, value);
        }

        public static readonly DependencyProperty BindableInlinesProperty =
            DependencyProperty.RegisterAttached(
                "BindableInlines",
                typeof(IEnumerable<Inline>),
                typeof(TextBlockHelper),
                new PropertyMetadata(null, OnBindableInlinesChanged));
        
        public static void OnBindableInlinesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBlock textBlock)
            {
                textBlock.Inlines.Clear();

                if (e.NewValue is IEnumerable<Inline> inlines)
                {
                    foreach (var inline in inlines)
                    {
                        textBlock.Inlines.Add(inline);
                    }
                }

                if (e.OldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= (s, args) => UpdateInlines(textBlock, args);
                }

                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += (s, args) => UpdateInlines(textBlock, args);
                }
            }
            
        }

        private static void UpdateInlines(TextBlock textBlock, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                textBlock.Inlines.Clear();
            }
            else if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Inline inline in args.NewItems)
                {
                    textBlock.Inlines.Add(inline);
                }
            }
        }
    }
}
