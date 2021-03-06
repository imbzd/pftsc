﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFTSModel;
using PFTSDesktop.Command;
using System.Windows.Input;
using PFTSUITemplate.Controls;
using System.Windows.Controls;
using System.Windows;
using PFTSTools;
using PFTSDesktop.View.OperatorManager;

namespace PFTSDesktop.ViewModel
{
    class OperatorManagerViewModel : WorkspaceViewModel
    {
        private static OperatorManagerViewModel instance;

        //操作员serviceobject
        private OperatorService OperatorService;

        private RelayCommand OperatorCommand;

        //操作员列表
        private List<@operator> OperatorList;
        //操作员信息
        private @operator OperatorInfo;

        //确认密码
        private string PasswordCFV;

        public OperatorManagerViewModel()
        {
            OperatorService = new OperatorService();

            OperatorInfo = new @operator();
        }

        public static OperatorManagerViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new OperatorManagerViewModel();
            }
            return instance;
        }

        /// <summary>
        /// 获取操作员列表
        /// </summary>
        public List<@operator> GetOperatorList
        {
            get {
                OperatorList = OperatorService.GetOperatorList();

                return OperatorList;
            }
            set
            {
                if (value == OperatorList)
                    return;

                OperatorList = value;

                base.OnPropertyChanged("GetOperatorList");
            }
        }

        /// <summary>
        /// 获取操作员信息
        /// </summary>
        public @operator GetOperatorInfo
        {
            get {
                return OperatorInfo;
            }
            set
            {
                if (value == OperatorInfo)
                    return;

                OperatorInfo = value;

                base.OnPropertyChanged("GetOperatorInfo");
            }
        }

        public string PasswordCF
        {
            get { return PasswordCFV; }
            set
            {
                if (value == PasswordCFV)
                    return;

                PasswordCFV = value;

                base.OnPropertyChanged("PasswordCF");
            }
        }

        /// <summary>
        /// 操作员管理命令
        /// </summary>
        public ICommand OperatorManageCommand
        {
            get
            {
                if (OperatorCommand == null)
                {
                    OperatorCommand = new RelayCommand(
                        new Action<Object>(this.OperatorManage)
                        );
                }
                return OperatorCommand;
            }
        }

        /// <summary>
        /// 操作员管理操作
        /// </summary>
        public void OperatorManage(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        /// <summary>
        /// 新增操作员命令
        /// </summary>
        public ICommand OperatorNewCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorNew)
                    );
            }
        }

        /// <summary>
        /// 新增操作员
        /// </summary>
        public void OperatorNew(Object obj)
        {
            OperatorInfo = new @operator();

            OperatorNewDlg dlg = new OperatorNewDlg();
            dlg.Show();
            //Button btn = (Button)obj;
            //Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        /// <summary>
        /// 新增操作员保存命令
        /// </summary>
        public ICommand OperatorNewSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorNewSave)
                    );
            }
        }

        /// <summary>
        /// 账号
        /// </summary>
        private bool CheckAccount()
        {
            //检查是否为空
            if (OperatorInfo.account == null || OperatorInfo.account.Trim().Length == 0)
            {
                MessageBox.Show("请填写账号！");

                return false;
            }

            OperatorInfo.account = OperatorInfo.account.Trim();

            //检查account是否已存在
            if (OperatorService.GetByAccount(OperatorInfo.account) != null)
            {
                MessageBox.Show("账号 " + OperatorInfo.account + " 已存在！");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 密码
        /// </summary>
        private bool CheckPassword()
        {
            //检查是否为空
            if (OperatorInfo.password == null || OperatorInfo.password.Trim().Length == 0)
            {
                MessageBox.Show("请填写密码！");

                return false;
            }

            OperatorInfo.password = OperatorInfo.password.Trim();

            return true;
        }

        /// <summary>
        /// 昵称
        /// </summary>
        private bool CheckName()
        {
            //检查是否为空
            if (OperatorInfo.name == null || OperatorInfo.name.Trim().Length == 0)
            {
                MessageBox.Show("请填写昵称！");

                return false;
            }

            OperatorInfo.name = OperatorInfo.name.Trim();

            return true;
        }

        /// <summary>
        /// 新增操作员保存
        /// </summary>
        /// <param name="obj"></param>
        public void OperatorNewSave(Object obj)
        {
            OperatorInfo = GetOperatorInfo;

            if (!CheckAccount()) return;
            if (!CheckPassword()) return;
            if (!CheckName()) return;

            //处理数据//
            //密码MD5加密
            OperatorInfo.password = MD5Tool.GetEncryptCode(OperatorInfo.password);

            bool Result = OperatorService.Insert(OperatorInfo);
            if (Result)
            {
                MessageBox.Show("保存成功！");

                GetOperatorList = OperatorService.GetOperatorList();
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                //Button btn = (Button)obj;
                //Global.currentFrame.NavigationService.GoBack();
            } else
            {
                MessageBox.Show("保存失败！");
            }
        }

        /// <summary>
        /// 编辑操作员命令
        /// </summary>
        public ICommand OperatorUpCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorUp)
                    );
            }
        }

        /// <summary>
        /// 编辑操作员保存命令
        /// </summary>
        private void OperatorUp(Object obj)
        {
            GetOperatorInfo.password = "";
            OperatorUpDlg dlg = new OperatorUpDlg();
            dlg.ShowDialog();
            //Button btn = (Button)obj;
            //Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        public ICommand OperatorUpSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorUpSave)
                    );
            }
        }

        /// <summary>
        /// 编辑操作员保存
        /// </summary>
        private void OperatorUpSave(Object obj)
        {
            OperatorInfo = GetOperatorInfo;
            if (OperatorInfo.id == 0 || OperatorInfo.id.ToString().Length == 0)
            {
                MessageBox.Show("未知操作员信息！");
                return;
            }
            
            if (OperatorInfo.password.Length > 0)
            {
                if (!CheckPassword()) return;
                if (PasswordCF != OperatorInfo.password)
                {
                    MessageBox.Show("确认密码不正确！");
                    return;
                }

                //密码MD5加密
                OperatorInfo.password = MD5Tool.GetEncryptCode(OperatorInfo.password);
            }

            if (!CheckName()) return;

            bool Result = OperatorService.OperatorUpInfo(OperatorInfo.id, OperatorInfo.password, OperatorInfo.name);
            if (Result)
            {
                MessageBox.Show("保存成功！");

                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                //Button btn = (Button)obj;
                //Global.currentFrame.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("保存失败！");
            }
        }
    }
}
