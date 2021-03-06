﻿using PFTSDesktop.Properties;
using PFTSModel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSDesktop.Model
{
    public class RFIDModel:IDataErrorInfo
    {
         public RFIDModel(string oldName, string oldNo)
        {
            this.OldName = oldName;
            this.OldNo = oldNo;
        }
        public RFIDModel()
        {

        }

        #region 变量
        private DevRFIDService service = new DevRFIDService();
        public string Name { get; set; }
        public string OldName { get; set; }
        public int Id { get; set; }
        public string No { get; set; }
        public string OldNo { get; set; }
        public int? Position_id { get; set; }
        #endregion

        #region IDataErrorInfo Members
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        #endregion

        #region 错误验证
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null)
                        return false;

                return true;
            }
        }

        static readonly string[] ValidatedProperties = 
        {
            "Name",
            "No",
        };

        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;
            switch (propertyName)
            {
                case "Name":
                    error = this.ValidateName();
                    break;

                case "No":
                    error = this.ValidateNo();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on VestModel: " + propertyName);
                    break;
            }
            return error;
        }

        private string ValidateName()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                return Resources.RFID_Error_MissName;
            }
            else if (IsValidReName(this.Name))
            {
                return Resources.RFID_Error_ReName;
            }
            return null;
        }

        private string ValidateNo()
        {
            if (String.IsNullOrEmpty(this.No))
            {
                return Resources.RFID_Error_MissNo;
            }
            else if (IsValidReNo(this.No))
            {
                return Resources.RFID_Error_ReNo;
            }
            return null;
        }

        private bool IsValidReName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;
            var vestModel = service.GetByName(name, OldName);
            return vestModel == null ? false : true;
        }

        private bool IsValidReNo(string no)
        {
            if (String.IsNullOrEmpty(no))
                return false;
            var vestModel = service.GetByNo(no, OldNo);
            return vestModel == null ? false : true;
        }
        #endregion
    }
}
