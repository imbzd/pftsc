﻿using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.View.DeviceManager;
using PFTSModel;
using PFTSModel.Services;
using PFTSUITemplate.Controls;
using PFTSUITemplate.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PFTSDesktop.ViewModel
{
    public class CameraManagerViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        #region 私有变量
        private RelayCommand _editCameraDlgCommand;
        private RelayCommand _addCamearDlgCommand;
        private CameraModel _cameraModel;
        private CameraModel _cameraAddModel;
        private RelayCommand _editCameraCommand;
        private RelayCommand _addCameraCommand;
        private view_camera_info _dev_camera;
        private static CameraManagerViewModel instance;
        private DevCameraService cameraService;
        private List<view_camera_info> _cameraList;
        private int type;
        private view_position_camera_info _positionModel;
        private List<view_position_camera_info> _positionList;
        private CameraPositionService positionService;
        #endregion

        public CameraManagerViewModel()
        {
            cameraService = new DevCameraService();
            positionService = new CameraPositionService();
            _dev_camera = new view_camera_info();
            _positionModel = new view_position_camera_info();
            _cameraAddModel = new CameraModel();
            _cameraModel = new CameraModel();
            initData();
        }

        public static CameraManagerViewModel GetInstance()
        {

            if (instance == null)
            {
                instance = new CameraManagerViewModel();
            }

            return instance;
        }

        #region command
        public ICommand AddCameraDlgCommand
        {
            get
            {
                if (_addCamearDlgCommand == null)
                {
                    _addCamearDlgCommand = new RelayCommand(
                        param => this.AddCameraDlg()
                        );
                }
                return _addCamearDlgCommand;
            }
        }

        public ICommand EditCameraDlgCommand
        {
            get
            {
                if (_editCameraDlgCommand == null)
                {
                    _editCameraDlgCommand = new RelayCommand(
                        param => this.EditCameraDlg()
                        );
                }
                return _editCameraDlgCommand;
            }
        }

        public ICommand EditCameraCommand
        {
            get
            {
                if (_editCameraCommand == null)
                {
                    _editCameraCommand = new RelayCommand(
                        new Action<Object>(this.EditCamera),
                        param => this.CanSave
                        );
                }
                return _editCameraCommand;
            }
        }

        public ICommand AddCameraCommand
        {
            get
            {
                if (_addCameraCommand == null)
                {
                    _addCameraCommand = new RelayCommand(
                        new Action<Object>(this.EditCamera),
                        param => this.AddCanSave
                        );
                }
                return _addCameraCommand;
            }
        }

        #endregion

        #region 属性
        public int PositionIndex
        {
            get;
            set;
        }

        public view_position_camera_info PositionModel
        {
            get
            {
                return _positionModel;
            }
            set
            {
                if (value == _positionModel || value == null)
                    return;
                _positionModel = value;
                _cameraModel.Position_id = _positionModel.id;
                _cameraAddModel.Position_id = _positionModel.id;
                base.OnPropertyChanged("PositionModel");
            }
        }

        public List<view_position_camera_info> PositionList
        {
            get
            {
                _positionList = positionService.GetPositionByStatus(false);
                if (type == 2)
                {
                    if (_dev_camera.position_id != null)
                    {
                        PositionModel = positionService.Get((int)_dev_camera.position_id);
                        if (PositionModel != null && PositionModel.id != 0)
                            _positionList.Insert(0, PositionModel);
                        PositionIndex = 0;
                    }
                    else
                    {
                        PositionModel = new view_position_camera_info();
                        PositionIndex = -1;
                    }
                }
                return _positionList;
            }
            set
            {
                if (value == _positionList)
                    return;
                _positionList = value;
                base.OnPropertyChanged("PositionList");
            }
        }

        public view_camera_info DevCamera
        {
            get
            {
                if (_dev_camera == null)
                {
                    _dev_camera = new view_camera_info();
                }
                return _dev_camera;
            }
            set
            {
                if (value == _dev_camera || value == null)
                    return;
                _dev_camera = value;
                _cameraModel.Id = _dev_camera.id;
                _cameraModel.Name = _dev_camera.name;
                _cameraModel.No = _dev_camera.no;
                _cameraModel.IP = _dev_camera.ip;
                _cameraModel.Port = _dev_camera.port.ToString();
                _cameraModel.Admin = _dev_camera.admin;
                _cameraModel.Password = _dev_camera.password;

                base.OnPropertyChanged("DevCamera");
            }
        }

        public CameraModel GetCameraModel
        {
            get
            {
                return _cameraModel;
            }
            set
            {
                if (value == _cameraModel)
                    return;
                _cameraModel = value;
                base.OnPropertyChanged("GetCameraModel");
            }
        }

        public CameraModel CameraAddModel
        {
            get
            {
                return _cameraAddModel;
            }
            set
            {
                if (value == _cameraAddModel)
                    return;
                _cameraAddModel = value;
                base.OnPropertyChanged("CameraAddModel");
            }
        }

        public List<view_camera_info> CameraList
        {
            get
            {
                return _cameraList;
            }
            set
            {
                if (value == _cameraList)
                    return;
                _cameraList = value;
                base.OnPropertyChanged("CameraList");
            }
        }

        #endregion

        #region 方法
        private void AddCameraDlg()
        {
            type = 1;
            CameraAddModel = new CameraModel();
            CameraAddDlg dlg = new CameraAddDlg();
            PositionModel = new view_position_camera_info();
            dlg.ShowDialog();
        }

        private void EditCameraDlg()
        {
            type = 2;
            CameraEditDlg dlg = new CameraEditDlg();
            _cameraModel.OldName = DevCamera.name;
            _cameraModel.OldNo = DevCamera.no;

            dlg.ShowDialog();
        }

        private void EditCamera(Object obj)
        {
            bool result = false;

            if (type == 1)
            {
                dev_camera model = new dev_camera();
                model.no = _cameraAddModel.No;
                model.name = _cameraAddModel.Name;
                model.ip = _cameraAddModel.IP;
                model.port = int.Parse(_cameraAddModel.Port);
                model.admin = _cameraAddModel.Admin;
                model.password = _cameraAddModel.Password;
                model.create_time = DateTime.Now;
                model.position_id = _cameraAddModel.Position_id == 0 ? null : _cameraAddModel.Position_id;
                result = cameraService.Insert(model);
            }
            else
            {
                _dev_camera.no = _cameraModel.No;
                _dev_camera.name = _cameraModel.Name;
                _dev_camera.ip = _cameraModel.IP;
                _dev_camera.port = int.Parse(_cameraModel.Port);
                _dev_camera.admin = _cameraModel.Admin;
                _dev_camera.password = _cameraModel.Password;
                _dev_camera.position_id = _cameraModel.Position_id == 0 ? null : _cameraModel.Position_id;
                result = cameraService.ModifyCamera(_dev_camera);
            }
            if (result)
            {
                MessageWindow.Show("摄像头信息更新成功！", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                initData();
            }
            else
            {
                MessageWindow.Show("摄像头信息更新失败！", "错误");
            }
        }

        bool CanSave
        {
            get { return _cameraModel.IsValid; }
        }

        bool AddCanSave
        {
            get { return _cameraAddModel.IsValid; }
        }
        #endregion



        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                if (type == 1)
                    return (_cameraAddModel as IDataErrorInfo).Error;
                else
                    return (_cameraModel as IDataErrorInfo).Error;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;
                if (type == 1)
                    error = (_cameraAddModel as IDataErrorInfo)[propertyName];
                else
                    error = (_cameraModel as IDataErrorInfo)[propertyName];

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
        #endregion // IDataErrorInfo Members

        #region 分页相关事件

        private RelayCommand _nextPageSearchCommand;
        public ICommand NextPageSearchCommand
        {
            get
            {
                if (_nextPageSearchCommand == null)
                {
                    _nextPageSearchCommand = new RelayCommand(
                            param => this.QueryData()
                            );
                }
                return _nextPageSearchCommand;
            }
        }
        private void QueryData()
        {
            CameraList = cameraService.GetPageByStatus(null, (PageIndex - 1) * PageSize, PageSize);
        }

        private void initData()
        {
            TotalCount = cameraService.GetCount(null);
            if (TotalCount == 0)
            {
                CameraList = new List<view_camera_info>();
            }
            else
            {
                CameraList = cameraService.GetPageByStatus(null, (PageIndex - 1) * PageSize, PageSize);
            }
        }
        #endregion
    }
}
