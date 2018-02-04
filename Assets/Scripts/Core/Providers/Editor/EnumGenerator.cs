using System.Collections.Generic;
using System.Text;
public static class EnumGenerator
{
    /// <summary>
    /// Get code of the enum
    /// </summary>
    /// <param name="nameNamespace">name for namespace where enum will be located</param>
    /// <param name="nameEnum">name for enum</param>
    /// <param name="items">names for items inside enum</param>
    /// <returns></returns>
    static public string GetCode(string nameNamespace, string nameEnum, IEnumerable<string> items)
    {
        const int capacity = 0x4FFF;

        var builder = new StringBuilder(capacity);

        builder.AppendLine("//This is the generated code. Don't modify.");
        builder.AppendLine("namespace " + nameNamespace);
        builder.AppendLine("{");
        builder.AppendLine("    public enum " + nameEnum);
        builder.AppendLine("    {");
        builder.AppendLine("        ");
        builder.AppendLine("        None = 0,");
        builder.AppendLine("        ");

        var index = 1;
        foreach (var item in items)
        {
            builder.AppendLine("        " + item + " = " + index + ",");

            ++index;
        }

        builder.AppendLine("    ");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();

        return builder.ToString();
    }
}