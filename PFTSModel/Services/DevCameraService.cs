﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class DevCameraService : Service<dev_camera>
    {
        /// <summary>
        /// 通过name获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_camera GetByName(string name, string oldName)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_camera> table = db.GetTable<dev_camera>();
                    var query = from dv in db.dev_camera
                                where dv.name == name
                                select dv;
                    if (!String.IsNullOrEmpty(oldName) && oldName == name)
                        return null;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 通过no获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_camera GetByNo(string no, string oldNo)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_camera> table = db.GetTable<dev_camera>();
                    var query = from dv in db.dev_camera
                                where dv.no == no
                                select dv;
                    if (!String.IsNullOrEmpty(oldNo) && oldNo == no)
                        return null;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }

        public bool ModifyLocker(dev_camera model)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    dev_camera camera = db.dev_camera.SingleOrDefault<dev_camera>(rec => rec.id == model.id);
                    camera.name = model.name;
                    camera.no = model.no;
                    camera.ip = model.ip;
                    camera.port = model.port;
                    camera.admin = model.admin;
                    camera.password = model.password;
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}