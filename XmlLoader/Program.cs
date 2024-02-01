using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace XmlLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serializer = new XmlSerializer(typeof(MappingRules));
            MappingRules rules = null;
            using (var fs = new FileStream(@".\TestMDS_BB70230_D02_CAT2_1R_old2.automap.xml", FileMode.Open))
            {
                var xmlReader = XmlReader.Create(fs);
                rules = (MappingRules)serializer.Deserialize(xmlReader);
            }           
            var mappingFile = XDocument.Load(@".\TestMDS_BB70230_D02_CAT2_1R_old2.automap.xml");
            
            foreach ((XElement mapping, string expression, string expression1) in from XElement mapping in mappingFile.Descendants("MappingRule")
                                                              where mapping.Descendants("MappingCondition").Descendants("MappingCondition").Any()
                                                              let sensorLocation = mapping.FirstAttribute.Value
                                                              let mappingRulesMappingRule = rules.MappingRule.Find(c => c.sensorlocation == sensorLocation)
                                                              let conditions = mappingRulesMappingRule.MappingCondition
                                                              let expression = GenerateExpression(conditions)
                                                              let expression1 = GenerateExpressionFromXml((XElement)mapping.Element("MappingCondition"))
                                                              select (mapping, expression, expression1))
            {
                mapping.Add(new XAttribute("MappingConditionString", expression));
                Console.WriteLine($"{expression}\n{expression1}");
            }
            
            foreach (XElement mapping in mappingFile.Descendants("MappingRule"))
            {
                
                mapping.Descendants("MappingCondition").Remove();
            }

            mappingFile.Save(@".\TestMDS_BB70230_D02_CAT2_1R_old1.automap.xml");
            Console.ReadLine();
        }

        public static string GenerateExpressionFromXml(XElement condition)
        {
            string boolOp = condition.Attribute("boolOp")?.Value;
            string boolExpression = "";

            if (boolOp == "AND" || boolOp =="OR")
            {
                boolExpression += "(";
            }

            boolExpression += string.Join($" {boolOp} ", condition.Elements()
                .Select(e => GenerateExpressionFromXml(e)));

            if (boolOp == "AND" || boolOp == "OR")
            {
                boolExpression += ")";
            }

            string type = condition.Attribute("type")?.Value;
            string param = condition.Attribute("param")?.Value;

            if (!string.IsNullOrEmpty(type))
            {
                boolExpression = type switch
                {
                    "SensorLocation" => $"(SensorLocation == \"{param}\")",
                    "LocationContainsText" => $"(Location.Contains(\"{param}\"))",
                    "LocationEqualsText" => $"(Location == \"{param}\")",
                    "LocationDirectionEqualsText" => $"(Direction == \"{param}\")",
                    "MatchMappingComment" => $"(MappingComment == \"{param}\")",
                    _ => throw new ArgumentException("Invalid condition type")
                };                
            }

            return boolExpression;
        }
        static string GenerateExpression(MappingConditionType condition)
        {
            StringBuilder builder = new StringBuilder();

            if (condition.MappingCondition != null && condition.MappingCondition.Count > 0)
            {
                builder.Append("(");
                for (int i = 0; i < condition.MappingCondition.Count; i++)
                {
                    builder.Append(GenerateExpression(condition.MappingCondition[i]));

                    if (i < condition.MappingCondition.Count - 1)
                    {
                        builder.Append($" {condition.boolOp} ");
                    }
                }
                builder.Append(")");
            }
            else
            {
                string text = condition.type switch
                {
                    "SensorLocation" => $"(SensorLocation == \"{condition.param}\")",
                    "LocationContainsText" => $"(Location.Contains(\"{condition.param}\"))",
                    "LocationEqualsText" => $"(Location == \"{condition.param}\")",
                    "LocationDirectionEqualsText" => $"(Direction == \"{condition.param}\")",
                    "MatchMappingComment" => $"(MappingComment == \"{condition.param}\")",
                    _ => throw new ArgumentException("Invalid condition type")
                };

                builder.Append(text);
            }

            return builder.ToString();
        }       
    }
    
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public abstract partial class MDSng_IdentifiableBase
{

    private string fileVersionIDField;

    public MDSng_IdentifiableBase()
    {
        this.fileVersionIDField = "0.0.0.0";
    }

    /// <remarks/>
    [System.ComponentModel.DefaultValueAttribute("0.0.0.0")]
    public string FileVersionID
    {
        get
        {
            return this.fileVersionIDField;
        }
        set
        {
            this.fileVersionIDField = value;
        }
    }
}


// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class MappingRules : MDSng_IdentifiableBase
{

    private System.Collections.Generic.List<MappingRulesMappingRule> mappingRuleField = new System.Collections.Generic.List<MappingRulesMappingRule>();

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("MappingRule")]
    public System.Collections.Generic.List<MappingRulesMappingRule> MappingRule
    {
        get
        {
            return this.mappingRuleField;
        }
        set
        {
            this.mappingRuleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class MappingRulesMappingRule
{

    private MappingConditionType mappingConditionField;

    private string mappingConditionStringField;

    private string sensorlocationField;

    /// <remarks/>
    public MappingConditionType MappingCondition
    {
        get
        {
            return this.mappingConditionField;
        }
        set
        {
            this.mappingConditionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string MappingConditionString
    {
        get
        {
            return this.mappingConditionStringField;
        }
        set
        {
            this.mappingConditionStringField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string sensorlocation
    {
        get
        {
            return this.sensorlocationField;
        }
        set
        {
            this.sensorlocationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "")]
public partial class MappingConditionType
{

    private System.Collections.Generic.List<MappingConditionType> mappingConditionField = new System.Collections.Generic.List<MappingConditionType>();

    private string typeField;

    private string boolOpField;

    private string paramField;

    public MappingConditionType()
    {
        this.typeField = "";
        this.boolOpField = "SINGLE";
        this.paramField = "";
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("MappingCondition")]
    public System.Collections.Generic.List<MappingConditionType> MappingCondition
    {
        get
        {
            return this.mappingConditionField;
        }
        set
        {
            this.mappingConditionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute("")]
    public string type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute("SINGLE")]
    public string boolOp
    {
        get
        {
            return this.boolOpField;
        }
        set
        {
            this.boolOpField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute("")]
    public string param
    {
        get
        {
            return this.paramField;
        }
        set
        {
            this.paramField = value;
        }
    }
}


