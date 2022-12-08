using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace あってるくんEP_2nd照合
{
    internal static class Program
    {
        public const string settingsFilePath = @"C:\Users\kamimoto\Desktop\work\開発\あってるくん EP 2nd照合\あってるくんEP 2nd照合\Settings\Settings.ini";
        public const string logFolderPath = @"C:\Users\kamimoto\Desktop\work\開発\あってるくん EP 2nd照合\あってるくんEP 2nd照合\Log\";
        private static string server;
        private static string port;
        private static string userName;
        private static string password;
        private static string dataBaseTokyo;
        private static string dataBaseOsaka;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        /// <summary>
        /// 設定ファイルから情報を取得するメソッド
        /// </summary>
        public static void GetSettingsInfo()
        {
            string line;
            try
            {
                // 設定ファイルを読み取り
                using (StreamReader sr = new StreamReader(settingsFilePath))
                {
                    // 一行ずつ全行読み込む
                    while ((line = sr.ReadLine()) != null)
                    {
                        // 各値を取得し変数に代入
                        if (line.Contains("Server") == true)
                        {
                            server = line.Substring(6).Replace("=", "").Replace(" ", "");
                        }
                        if (line.Contains("Port") == true)
                        {
                            port = line.Substring(4).Replace("=", "").Replace(" ", "");
                        }
                        if (line.Contains("UserName") == true)
                        {
                            userName = line.Substring(8).Replace("=", "").Replace(" ", "");
                        }
                        if (line.Contains("Password") == true)
                        {
                            password = line.Substring(8).Replace("=", "").Replace(" ", "");
                        }
                        if (line.Contains("DataBaseTokyo") == true)
                        {
                            dataBaseTokyo = line.Substring(13).Replace("=", "").Replace(" ", "");
                        }
                        if (line.Contains("DataBaseOsaka") == true)
                        {
                            dataBaseOsaka = line.Substring(13).Replace("=", "").Replace(" ", "");
                        }
                    }
                }
            }
            catch
            {
                // 呼び出し元に例外を投げる
                throw;
            }
        }


        /// <summary>
        /// ログファイルを作成・書き込みするメソッド
        /// </summary>
        public static void WriteTraceLog(string subject, string msg)
        {
            // ログフォルダがなければ作成する
            if (Directory.Exists(logFolderPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(logFolderPath);
                    Debug.WriteLine("ログフォルダ作成成功:" + logFolderPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ログフォルダ作成失敗\n" + ex);
                    MessageBox.Show("ログフォルダの作成に失敗しました。\n処理は続行されますが、システム管理者に確認してください。\n\n" + ex,
                                    "エラー",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }

            // 書き込み先ログファイル情報を取得
            DateTime date = DateTime.Now;
            string dateStr = date.ToString("yyyyMMdd");
            string logFilePath = logFolderPath + dateStr + ".log";

            // ログファイルがなければ作成
            if (File.Exists(logFilePath) == false)
            {
                try
                {
                    using (File.Create(logFilePath))
                    {
                        Debug.WriteLine("ログファイル作成成功:" + logFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ログファイル作成失敗\n" + ex);
                    MessageBox.Show("ログファイルの作成に失敗しました。\n処理は続行されますが、システム管理者に確認してください。\n\n" + ex,
                                    "エラー",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }

            // ログファイルに書き込む
            try
            {
                Encoding enc = Encoding.GetEncoding("UTF-8");
                using (StreamWriter sw = new StreamWriter(logFilePath, true, enc))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",[" + subject + "]:" + msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ログファイル書き込み失敗\n" + ex);
                MessageBox.Show("ログファイルの書き込みに失敗しました。\n処理は続行されますが、システム管理者に確認してください。\n\n" + ex,
                                "エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 過去のログファイルを削除するメソッド
        /// </summary>
        public static void DeleteTraceLog()
        {
            // 削除対象ファイル(先々月分)の情報を取得
            DateTime date = DateTime.Now;
            string monthBeforeLast = date.AddMonths(-2).ToString("yyyyMM");
            DirectoryInfo di = new DirectoryInfo(logFolderPath);

            // 削除対象のファイルをすべて削除する
            try
            {
                foreach (FileInfo fi in di.EnumerateFiles(monthBeforeLast + "*.log"))
                {
                    string fileName = fi.Name;
                    File.Delete(logFolderPath + fileName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ログファイル削除失敗\n" + ex);
                MessageBox.Show("古いログファイルを削除できませんでした\n\n" + ex);
            }
        }
    }
}
