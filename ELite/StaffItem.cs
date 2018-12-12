using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace ELite
{
    #region ENUM

    public enum Sex
    {
        保密,
        男,
        女,
        百分比设置_未实现,
        标签设置_未实现,
        自定义
    }

    public enum StaffIdentity
    {
        老板,
        店长,
        主管,
        前台白班员工,
        夜班前台员工,
        吧台员工,
        旅游咨询,
        实习生_主管,
        实习生_前台_白班,
        实习生_前台_夜班,
        实习生_吧台,
        实习生_旅游,
        主管实习生,
        前台白班实习生,
        夜班前台实习生,
        吧台实习生,
        旅游咨询实习生,
        志愿者,
        过去
    }

    #endregion
    
    public struct StaffItemCollection
    {
        public int SID { get; set; }
        public string Nickname { get; set; }
        public string Surname { get; set; }
        public string Givenname { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birth { get; set; }
        public StaffIdentity Identity { get; set; }
    }

    public class StaffItem
    {
        public StaffItemCollection Items;
        
        public void Load()
        {

        }

        private void Load(DataRow row)
        {
            Items.SID = (int)row[0];
            Items.Nickname = row[1].ToString();
            Items.Surname = row[2].ToString();
            Items.Givenname = row[3].ToString();
            Items.Sex = (Sex)(row[4]);
            Items.Birth = (DateTime)row[5];
            Items.Identity = (StaffIdentity)row[6];
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(StaffItem)) return false;
            return Items.SID == (obj as StaffItem).Items.SID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
