﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PFTSHwCtrl
{
    public enum VideoRecordEvent
    {
        StartRecord,
        EndRecord
    }

    #region Handler
    public delegate void VideoRecordHandler(PFTSModel.dev_camera camera, VideoRecordEvent e);
    #endregion

    public class PFTSVideoRecordProxy
    {
        private Dictionary<int, Room> m_mapRooms = new Dictionary<int, Room>();

        public event VideoRecordHandler VideoRecordDelegate;

        public PFTSVideoRecordProxy()
        {
            LoadRooms();
        }

        private void LoadRooms()
        {
            List<PFTSModel.view_camera_info> cameras = (new PFTSModel.Services.DevCameraService()).GetPageByStatus(true,0,100);
            List<PFTSModel.btracker> btrackers = (new PFTSModel.Services.BTrackerService()).GetAllInscene();
            Dictionary<int, List<PFTSModel.btracker>> mapBtrackers = new Dictionary<int, List<PFTSModel.btracker>>();
            foreach(var b in btrackers)
            {
                if (b.room_id == null) continue;
                if (mapBtrackers.ContainsKey(b.room_id.Value))
                {
                    mapBtrackers[b.room_id.Value].Add(b);
                }else
                {
                    var list = new List<PFTSModel.btracker>();
                    list.Add(b);
                    mapBtrackers.Add(b.room_id.Value, list);
                }
            }
            foreach (var ca in cameras)
            {
                if (ca.room_id == null) continue;
                List<PFTSModel.btracker> list = null;
                if (mapBtrackers.ContainsKey(ca.room_id.Value))
                {
                    list = mapBtrackers[ca.room_id.Value];
                }
                var room = new Room(ca,list);
                if (VideoRecordDelegate != null)
                {
                    room.VideoRecordDelegate += VideoRecordDelegate;
                }
                m_mapRooms.Add(ca.room_id.Value, room);
            }
        }

        public void BtrackerMoveTo(int btrackerId,int? fromRoomId,int toRoomId)
        {
            if (fromRoomId != null)
            {
                if (m_mapRooms.ContainsKey(fromRoomId.Value))
                {
                    m_mapRooms[fromRoomId.Value].BtrackerOut(btrackerId);
                }
            }
            var btracker = (new PFTSModel.Services.BTrackerService()).Get(btrackerId);
            if (btracker == null)
            {
                MessageBox.Show("查找嫌疑人信息失败");
                return;
            }
            if (m_mapRooms.ContainsKey(toRoomId))
            {
                m_mapRooms[toRoomId].BtrackerIn(btracker);
            }
        }

        public class Room
        {
            private PFTSModel.view_camera_info m_camera;
            private List<PFTSModel.btracker> m_btrackers;
            public event VideoRecordHandler VideoRecordDelegate;
            private bool m_bRecoding = false;

            public Room(PFTSModel.view_camera_info camera, List<PFTSModel.btracker> btrackers)
            {
                m_camera = camera;
                if (btrackers == null)
                {
                    m_btrackers = new List<PFTSModel.btracker>();
                }else
                {
                    m_btrackers = btrackers;
                }
                StartRecord();
            }

            /// <summary>
            /// 开始录制
            /// </summary>
            public void StartRecord()
            {
                Thread recordThread = new Thread(new ThreadStart(Record));
                recordThread.Start();
            }

            private void Record()
            {
                if (m_bRecoding == false && m_btrackers.Count > 0)
                {
                    Console.WriteLine("房间(" + m_camera.room_name + ")" + "开始录制");
                    m_bRecoding = true;
                }
            }

            public void BtrackerIn(PFTSModel.btracker bt)
            {
                m_btrackers.Add(bt);
                if (!m_bRecoding)
                {
                    Console.WriteLine(bt.name + "进入了"+"房间(" + m_camera.room_name + ")" + "开始录制");
                    m_bRecoding = true;
                }
            }

            public void BtrackerOut(int btId)
            {
                PFTSModel.btracker btr = null;
                foreach (var bt in m_btrackers)
                {
                    if (bt.id == btId)
                    {
                        btr = bt;
                        m_btrackers.Remove(bt);
                        break;
                    }
                }
                if (m_bRecoding && m_btrackers.Count == 0)
                {
                    string str = "";
                    if (btr != null)
                    {
                        str += btr.name + "离开了" + "房间(" + m_camera.room_name + ")";
                    }else
                    {
                        str += "房间(" + m_camera.room_name + ")没人";
                    }
                    Console.WriteLine(str + ",停止录制");
                }
            }
        }
    }
}