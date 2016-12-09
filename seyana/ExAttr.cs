using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonmile.ExDoc
{
    public class ExAttr
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ExElement Parent { get; set; }

    }
    public class ExAttrs : List<ExAttr>
    {
        public static implicit operator ExAttr(ExAttrs lst)
        {
            return lst.First();
        }
        public static implicit operator string(ExAttrs lst)
        {
            return lst.First().Value;
        }
        public static implicit operator List<string>(ExAttrs lst)
        {
            return null;
        }

        /// <summary>
        /// 属性を名前で抽出する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExAttrs SelectAttrs(string name)
        {
            var lst = new ExAttrs();
            foreach (var attr in this)
            {
                if (attr.Name == name)
                {
                    lst.Add(attr);
                }
            }
            return lst;
        }

        /// <summary>
        /// 属性を値で抽出する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExAttrs SelectAttrsByValue(string value, bool reverse = false)
        {
            var lst = new ExAttrs();
            foreach (var attr in this)
            {
                if (reverse == false)
                {
                    if (attr.Value == value)
                    {
                        lst.Add(attr);
                    }
                }
                else
                {
                    if (attr.Value != value)
                    {
                        lst.Add(attr);
                    }
                }
            }
            return lst;
        }

        public static ExAttrs operator == ( ExAttrs lst, string value )
        {
            return lst.SelectAttrsByValue( value );
        }
        public static ExAttrs operator != ( ExAttrs lst, string value )
        {
            return lst.SelectAttrsByValue(value, true);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
