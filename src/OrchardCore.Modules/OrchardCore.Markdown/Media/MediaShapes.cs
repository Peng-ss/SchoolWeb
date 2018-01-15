using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;

namespace OrchardCore.Markdown.Media
{
    public class MediaShapes : IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("Markdown_Editor")
                .OnDisplaying(displaying =>
                {
                    IShape editor = displaying.Shape;

                    if (editor.Metadata.Alternates.Contains("Markdown_Editor__Wysiwyg"))
                    {
                        editor.Metadata.Wrappers.Add("Media_Wrapper__Markdown");
                    }
                });
        }
    }
}
