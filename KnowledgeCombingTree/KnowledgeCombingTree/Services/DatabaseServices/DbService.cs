﻿using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using KnowledgeCombingTree.Models;
using Windows.UI.Popups;

namespace KnowledgeCombingTree.Services.DatabaseServices
{
    class DbService
    {
        private static string DB_NAME = "KnowledgeCombingTree.db";
        private static string CREATE_DB = @"CREATE TABLE IF NOT EXISTS
                                            treenodes(id             VARCHAR(40) PRIMARY KEY NOT NULL,
                                                      parent_id      VARCHAR(40) NOT NULL,
                                                      level          INTEGER NOT NULL,
                                                      path           VARCHAR(60) NOT NULL,
                                                      name           VARCHAR(20) NOT NULL,
                                                      description    VARCHAR(200),
                                                      image          VARCHAR(60) NOT NULL
                                                     );";
        private static string SELECT_ITEM = @"SELECT id, parent_id, level, path, name, description, image
                                                FROM treenodes
                                                WHERE id = ?";
        private static string SELECT_ITEMS = @"SELECT id FROM treenodes";
        private static string SELECT_ITEMS_BY_ID = @"SELECT id, parent_id, level, path, name, description, image
                                                    FROM treenodes
                                                    WHERE id = ?";
        private static string SELECT_ITEMS_BY_PARENT_ID = @"SELECT id, parent_id, level, path, name, description, image
                                                    FROM treenodes
                                                    WHERE parent_id = ?";
        private static string ADD_ITEM = @"INSERT INTO treenodes (id, parent_id, level, path, name, description, image)
                                                VALUES(?, ?, ?, ?, ?, ?, ?)";
        private static string UPDATE_ITEM = @"UPDATE treenodes
                                                SET level = ?, path = ?, name = ?, description = ?, image = ?
                                                WHERE id = ?";
        private static string DELETE_ITEM = @"DELETE FROM treenodes WHERE id = ?";
        private static string EXIST = @"SELECT id FROM treenodes WHERE id = ?";

        private static string LAST_INSERTED_ID = @"SELECT last_insert_rowid()";

        private static SQLiteConnection conn = GetConnection();

        private static SQLiteConnection GetConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(DB_NAME);
            using (var statement = conn.Prepare(CREATE_DB))
            {
                statement.Step();
            }
            return conn;
        }

        // get a item by id
        public static TreeNode GetItem(string id)
        {
            TreeNode item = null;
            using (var statement = conn.Prepare(SELECT_ITEM))
            {
                statement.Bind(1, id);
                if (SQLiteResult.ROW == statement.Step())
                {
                    item = new TreeNode((string)statement[0],
                                        (string)statement[1],
                                        (long)statement[2],
                                        (string)statement[3],
                                        (string)statement[4],
                                        (string)statement[5],
                                        (string)statement[6]);
                }
            }
            return item;
        }

        // get all items
        public static ObservableCollection<TreeNode> GetItems()
        {
            ObservableCollection<TreeNode> list = new ObservableCollection<TreeNode>();
            using (var statement = conn.Prepare(SELECT_ITEMS))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    try
                    {
                        string id = (string)statement[0];
                        list.Add(GetItem(id));
                    }
                    catch (Exception e)
                    {
                        var i = new MessageDialog(e.Message).ShowAsync();
                    }
                }
            }
            return list;
        }

        // get items by parent's id
        // if parent_id == "-1", select root nodes, otherwise, select child nodes
        public static ObservableCollection<TreeNode> GetItemsByParentId(string parent_id)
        {
            ObservableCollection<TreeNode> list = new ObservableCollection<TreeNode>();
            using (var statement = conn.Prepare(SELECT_ITEMS_BY_PARENT_ID))
            {
                statement.Bind(1, parent_id);
                while (SQLiteResult.ROW == statement.Step())
                {
                    try
                    {
                        TreeNode item = new TreeNode((string)statement[0],
                                                     (string)statement[1],
                                                     (long)statement[2],
                                                     (string)statement[3],
                                                     (string)statement[4],
                                                     (string)statement[5],
                                                     (string)statement[6]);
                        list.Add(item);
                    }
                    catch (Exception e)
                    {
                        var i = new MessageDialog(e.Message).ShowAsync();
                    }
                }
            }
            return list;
        }

        // tell whether an item exists or not
        public static bool Exist(string id)
        {
            bool flag = false;
            using (var statement = conn.Prepare(EXIST))
            {
                statement.Bind(1, id);
                if (SQLiteResult.ROW == statement.Step())
                {
                    flag = (string)statement[0] == "" ? false : true;
                }
            }
            return flag;
        }

        // add a tree node into datebase
        public static void AddItem(TreeNode item)
        {
            using (var statement = conn.Prepare(ADD_ITEM))
            {
                statement.Bind(1, item.getId());
                statement.Bind(2, item.getParentId());
                statement.Bind(3, item.getLevel());
                statement.Bind(4, item.getPath());
                statement.Bind(5, item.getName());
                statement.Bind(6, item.getDescription());
                statement.Bind(7, item.getImage());
                statement.Step();
            }

            //using (var statement = conn.Prepare(LAST_INSERTED_ID))
            //{
            //    return statement[0] == null ? -1 : (long)statement[0];
            //}
            //return -1;
        }

        public static void UpdateItem(TreeNode item)
        {
            if (Exist(item.getId()))
            {
                using (var statement = conn.Prepare(UPDATE_ITEM))
                {
                    statement.Bind(1, item.getLevel());
                    statement.Bind(2, item.getPath());
                    statement.Bind(3, item.getName());
                    statement.Bind(4, item.getDescription());
                    statement.Bind(5, item.getImage());
                    statement.Bind(6, item.getId());
                    statement.Step();
                }
            }
        }

        public static void DeleteItem(string id)
        {
            using (var statement = conn.Prepare(DELETE_ITEM))
            {
                statement.Bind(1, id);
                statement.Step();
            }
        }
    }
}
