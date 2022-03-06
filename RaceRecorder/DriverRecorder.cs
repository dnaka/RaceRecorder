using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace RaceRecorder
{
    enum State
    {
        RESISTING,  // 初期状態で登録中またはレース終了後
        WAIT_START, // 登録完了してスタート待
        RACING      // レース中
    }



    public partial class RaceRecorder : Form
    {
        private DataTable driverTable;
        private DateTime startTime;
        private State state = State.RESISTING;
        private Dictionary<string, List<string>> records;
        private string driverCache;
        private string recordsCache;

        private static string NO = "ゼッケン";
        private static string NAME = "氏名";
        private static string LAP = "LAP数";
        private static string LAPTIME = "最終LapTime";

        public RaceRecorder()
        {
            InitializeComponent();
            this.records = new Dictionary<string, List<string>>();

            this.driverCache = Path.GetTempPath() + "DriverRecorderCache.csv";
            this.recordsCache = Path.GetTempPath() + "DriverRecorderRecordsCache.csv";
        }

        private void Form_Load(object sender, EventArgs e)
        {
            setupDriverList();
            setControlFromState(State.RESISTING);
        }

        private void setupDriverList()
        {
            this.driverTable = new DataTable("driverList");
            this.driverTable.Columns.Add(RaceRecorder.NO, Type.GetType("System.String"));
            this.driverTable.Columns.Add(RaceRecorder.NAME, Type.GetType("System.String"));
            this.driverTable.Columns.Add(RaceRecorder.LAP, Type.GetType("System.UInt32"));
            this.driverTable.Columns.Add(RaceRecorder.LAPTIME, Type.GetType("System.String"));
            this.driverTable.Columns[2].ReadOnly = true;
            this.driverTable.Columns[3].ReadOnly = true;
            loadCache();
            DriverListView.DataSource = this.driverTable;
        }

        private void loadCache()
        {
            try
            {
               ;

                if (File.Exists(this.driverCache) && File.Exists(this.recordsCache))
                {
                    // driverTableの生成
                    string[] lines = File.ReadAllLines(this.driverCache);
                    this.RaceNameTextBox.Text = lines[0].Split(',')[0];
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] cols = lines[i].Split(',');
                        DataRow r = this.driverTable.NewRow();
                        r.SetField(0, cols[0]);
                        r.SetField(1, cols[1]);
                        r.SetField(2, Int32.Parse(cols[2]));
                        r.SetField(3, cols[3]);
                        this.driverTable.Rows.Add(r);
                    }

                    // records側の生成
                    this.records = new Dictionary<string, List<string>>();
                    string[] lines2 = File.ReadAllLines(this.recordsCache);

                    for (int i = 0; i < lines2.Length; i++)
                    {
                        string[] cols = lines2[i].Split(',');

                        List<string> record = new List<string>(); 
                        for (int j = 1; j < cols.Length; j++)
                        {
                            record.Add(cols[j]);
                        }
                        this.records.Add(cols[0].ToString(), record);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.clearCache();
            }
        }

        // エラー時などやクリアボタン押下時にデータを初期化する
        private void clearCache()
        {
            this.driverTable.Clear();
            this.records.Clear();
            this.RaceNameTextBox.Text = "";

            try
            {
                File.Delete(this.driverCache);
                File.Delete(this.recordsCache);
            } 
            catch (Exception e) 
                {
                Console.WriteLine("Cache Remove failed, " + e.Message);
            }

            setControlFromState(State.RESISTING);

        }

        private void setControlFromState(State toState)
        {
            switch (toState)
            {
                case State.RESISTING:
                    // 起動後か、RACING中にスタートをおしてタイマーを止めたらここ
                    DriverListView.Enabled = true;
                    validateButton.Enabled = true;
                    startButton.Enabled = false;
                    resultViewButton.Enabled = true;
                    StateLabel.Text = "ドライバー登録中/レース終了";
                    registButton.Enabled = false;
                    state = State.RESISTING;
                    break;

                case State.WAIT_START:
                    // 確定ボタンを押したらここ
                    DriverListView.Enabled = true;
                    validateButton.Enabled = true;
                    startButton.Enabled = true;
                    resultViewButton.Enabled = true;
                    StateLabel.Text = "スタート可能";
                    startButton.Text = "レーススタート";
                    registButton.Enabled = false;
                    resetLapTime();
                    state = State.WAIT_START;
                    break;
            
                case State.RACING:
                    // スタートボタンを推してタイマーを開始したらここ
                    DriverListView.Enabled = false;
                    validateButton.Enabled = false;
                    startButton.Enabled = true;
                    resultViewButton.Enabled = false;
                    registButton.Enabled = true;
                    StateLabel.Text = "レース中";
                    startButton.Text = "レース停止";
                    DriverNumberTextBox.Focus();
                    state = State.RACING;
                    break;
            }
        }

        //スタート時にラップタイムの入力をリセットする
        private void resetLapTime()
        {
            TimeLabel.Text = "00:00:00";
            this.driverTable.Columns[2].ReadOnly = false;
            this.driverTable.Columns[3].ReadOnly = false;
            for (int i = 0; i < this.driverTable.Rows.Count; i++)
            {
                if (this.driverTable.Rows[i][0] != null &&
                    driverTable.Rows[i][1] != null)
                {
                    this.driverTable.Rows[i][0] = this.driverTable.Rows[i][0].ToString().Trim();
                    this.driverTable.Rows[i][1] = this.driverTable.Rows[i][1].ToString().Trim();
                    this.driverTable.Rows[i][2] = 0;
                    this.driverTable.Rows[i][3] = "00:00:00";
                }
            }
            this.driverTable.Columns[2].ReadOnly = true;
            this.driverTable.Columns[3].ReadOnly = true;
        }

        private bool validateDriverInfo()
        {
            int i = 0;
            DataRowCollection c = this.driverTable.Rows;
            while (i < c.Count)
            {

                object noCell = this.driverTable.Rows[i][0];
                object nameCell = this.driverTable.Rows[i][1];

                bool isNoNull = (noCell == null || string.IsNullOrEmpty(noCell.ToString()));
                bool isNameNull = (nameCell == null || string.IsNullOrEmpty(nameCell.ToString()));

                // 両方ない行は削除
                if ((!isNoNull && isNameNull) || (isNoNull && !isNameNull))
                {
                    // アイコンも指定
                    MessageBox.Show("ゼッケンか名前が未入力の選手がいます", "ドライバー登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                Regex re = new Regex("[０-９]+");
                string no2 = re.Replace(noCell.ToString(), myReplacer);

                int n;
                if (!Int32.TryParse(no2, out n))
                {
                    MessageBox.Show("ゼッケンに数字以外が設定されています", "ドライバー登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.driverTable.Rows[i][0] = no2;
                i++;     
            }

            for (i = 0; i < this.driverTable.Rows.Count; i++)
            {
                for (int j = i + 1; j < this.driverTable.Rows.Count; j++)
                {
                    if (this.driverTable.Rows[i][0].ToString().Equals(this.driverTable.Rows[j][0].ToString()))
                    {
                        // アイコンも指定
                        MessageBox.Show("同じゼッケンの選手がいます", "ドライバー登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        // ドライバー登録後の登録完了ボタン押下
        private void validateButton_Click(object sender, EventArgs e)
        {
            if(validateDriverInfo())
            {
                setControlFromState(State.WAIT_START);

                // recordsを初期化
                this.records = new Dictionary<string, List<string>>();
                foreach (DataRow r in this.driverTable.Rows)
                {
                    this.records.Add(r[0].ToString(), new List<string>());
                }
                // キャッシュ更新
                saveCache();
            }
            else
            {
                setControlFromState(State.RESISTING);
            }            
        }

        // スタートボタン押下イベント
        private void startButton_Click(object sender, EventArgs e)
        {
            if (state == State.WAIT_START) {
                setControlFromState(State.RACING);
                startTime = DateTime.Now;
                lapTimer.Enabled = true;
            }
            else
            {
                lapTimer.Enabled = false;
                saveCache();
                setControlFromState(State.RESISTING);
            }
        }


        // ラップ通過記録ボタン押下イベント
        private void registButton_Click(object sender, EventArgs e)
        {
            string no = DriverNumberTextBox.Text;
            // 全角->半角変換
            Regex re = new Regex("[０-９]+");
            string no2 = re.Replace(no, myReplacer);

            //整数の時だけ登録
            int n;
            if (Int32.TryParse(no2, out n))
            {
                regist(no2);
            }

            DriverNumberTextBox.Text = "";
            DriverNumberTextBox.Focus();
        }

        // 記録の登録処理
        private void regist(string no)
        {
            try
            {
                if (string.IsNullOrEmpty(no))
                {
                    return;
                }

                for (int i = 0; i < this.driverTable.Rows.Count; i++)
                {
                    // 文字列として一致するゼッケンを探す
                    if (this.driverTable.Rows[i][0].ToString() == no)
                    {
                        this.driverTable.Columns[2].ReadOnly = false;
                        this.driverTable.Columns[3].ReadOnly = false;
                        this.driverTable.Rows[i][2] = UInt32.Parse(DriverListView.Rows[i].Cells[2].Value.ToString()) + 1;
                        this.driverTable.Rows[i][3] = getTimeAfterStart();
                        this.driverTable.Columns[2].ReadOnly = true;
                        this.driverTable.Columns[3].ReadOnly = true;

                        this.records[no].Add(this.driverTable.Rows[i][3].ToString());

                        break;
                    }
                }

                this.driverTable.DefaultView.Sort = RaceRecorder.LAP + " DESC, " + RaceRecorder.LAPTIME + " ASC";
                this.driverTable = this.driverTable.DefaultView.ToTable(true);
                DriverListView.DataSource = this.driverTable;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return;
            }
        }

        // スタート後のタイマー表示更新
        private void lapTimer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = getTimeAfterStart();
        }

        // スタート後の経過時間テキストを取得する
        private string getTimeAfterStart()
        {
            if (state == State.RACING)
            {
                TimeSpan diff = DateTime.Now - startTime;
                return diff.Hours.ToString("D2") + ":" + diff.Minutes.ToString("D2") + ":" + diff.Seconds.ToString("D2");
            }
            return "00:00:00";
        }

        // ゼッケン番号テキストボックスでEnterを推したらラップタイムの登録にかかる
        private void DriverNumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && this.state == State.RACING)
            {
                regist(DriverNumberTextBox.Text);
            }
            DriverNumberTextBox.Text = "";
        }

        // 結果表示
        private void resultViewButton_Click(object sender, EventArgs e)
        {
            string tmpFile =  makeResultHtml();

            string url = "file:///" + tmpFile.Replace("\\", "/");
            System.Diagnostics.Process.Start(url);
        }

        private void saveCache()
        {
            string tmpFile = Path.GetTempPath() + "DriverRecorderCache.csv";
            StringBuilder sb = new StringBuilder(this.RaceNameTextBox.Text + ",,,\r\n");
            foreach (DataRow r in this.driverTable.Rows)
            {
                sb.Append(r[0] +"," + r[1] +"," + r[2] + "," + r[3] + "\r\n");
            }
            File.WriteAllText(tmpFile, sb.ToString());


            // 個々のレコードの保存
            string tmpFile2 = Path.GetTempPath() + "DriverRecorderRecordsCache.csv";
            StringBuilder recordBuffer = new StringBuilder();
            foreach (string key in this.records.Keys)
            {
                StringBuilder line = new StringBuilder(key + ",");
                foreach (string record in this.records[key])
                {
                    line.Append(record + ",");
                }

                // 末尾についている,を削除
                recordBuffer.Append(line.ToString().TrimEnd(',') + "\r\n");
            }
            File.WriteAllText(tmpFile2, recordBuffer.ToString());
        }


        // 30行のテーブル1個を作る
        private string makeOneTable(int startIndex)
        {
            if (startIndex > this.driverTable.Rows.Count - 1)
            {
                return ""; 
            }

            int endIndex = startIndex + 29;
            if (endIndex > this.driverTable.Rows.Count - 1)
            {
                endIndex = this.driverTable.Rows.Count - 1;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
            {
                DataRow r = this.driverTable.Rows[i];
                string recordPath = Path.GetTempPath() + r[0] + ".html";
                int rank = i + 1;
                sb.Append(
                   "<tr><td align=\"center\">" + rank.ToString() +
                   "</td><td align=\"center\">" + r[0].ToString() +
                   "</td><td align=\"center\"><a href=\"" + recordPath + "\">" + r[1].ToString() +
                   "</a></td><td align=\"center\">" + r[2].ToString() +
                   "</td><td align=\"center\">" + r[3].ToString() + "</td></tr>\r\n"
               );

               makeRecordhtml(r[0].ToString(), r[1].ToString(), recordPath);
            }

            string table =
                "<table border=\"1\" width=\"100%\">\r\n" +
                "<tr><th>順<br>位</th><th>" +
                //RaceRecorder.NO +
                "ゼッ<br>ケン" +
                "</th><th>" +
                //RaceRecorder.NAME +
                "氏名" +
                "</th><th>" +
                //RaceRecorder.LAP +
                "Lap<br>数" +
                "</th><th>" +
                //RaceRecorder.LAPTIME +
                "最終<br>LapTime" +
                "</th></tr>\r\n" +
                sb.ToString() +
                "</table>\r\n";

            return table;
        }
        
        // HTMLファイルを生成してtmpファイルを作る
        private string makeResultHtml()
        {
            string tmpPath = Path.GetTempPath() + "result.html";            

            int listNum = this.driverTable.Rows.Count % 30 == 0 ? this.driverTable.Rows.Count / 30 : this.driverTable.Rows.Count / 30 + 1;
            int pageNum = listNum % 2 == 0 ? listNum / 2: listNum / 2 + 1;

            string date  = "<h3>" + DateTime.Now.ToLocalTime().ToShortDateString() + "</h3>\r\n";
            string title = "<h1>" + this.RaceNameTextBox.Text + " 総合順位</h1>\r\n";

            StringBuilder sb = new StringBuilder();
            for (int page = 0; page < pageNum; page++)
            {
                sb.Append(date);
                sb.Append(title);

                if (listNum == 1)
                {
                    string table = makeOneTable(page * 2 * 30);
                    string div =
                        "<table width=\"100%\"><tr>" + 
                        "<td width=\"48%\">" + table + "</td>" + 
                        "<td width=\"2%\"></td>" +
                        "<td width=\"48%\"></td></tr></table>";
                    sb.Append(div);
                }
                else
                {
                    string table1 = makeOneTable(page * 2 * 30);
                    string table2 = makeOneTable(page * 2 * 30 + 30);

                    string div =
                        "<table width=\"100%\"><tr>" + 
                        "<td width=\"48%\">" + table1 + "</td>" +
                        "<td width=\"2%\"></td>" +
                        "<td width=\"48%\">" + table2 + "</td></tr></table>\r\n";
                    sb.Append(div);
                }
                sb.Append("<div class=\"pagebreak\"></div>\r\n");
            }

            string style = ".pagebreak { break-after: page; }";
            string header = "<header>\r\n<title>" + this.RaceNameTextBox.Text +"</title>\r\n<style>\r\n" + style + "</style>\r\n</header>\r\n";
            string html = header + "<body>\r\n" + sb.ToString() + "\r\n</body>";
            File.WriteAllText(tmpPath, html);

            return tmpPath;
        }

        private void makeRecordhtml(string no, string name, string path)
        {
            int tableNum = 3; // 1ページの表の個数
            int rowNum = 30;  // 表1個当たりの行の数

            List<string> record = this.records[no];
            int listNum = record.Count % rowNum == 0 ? record.Count / rowNum : record.Count / rowNum + 1;
            int pageNum = listNum % tableNum == 0 ? listNum / tableNum : listNum / tableNum + 1;

            string date = "<h3>" + DateTime.Now.ToLocalTime().ToShortDateString() + "</h3>\r\n";
            string title = "<h2>" + this.RaceNameTextBox.Text + " 個人成績</h2>\r\n";
            string rider = "<h1>" + no + "：" + name + "</h1>\r\n";

            StringBuilder sb = new StringBuilder();
            if (listNum > 0)
            {
                for (int page = 0; page < pageNum; page++)
                {
                    sb.Append(date);
                    sb.Append(title);
                    sb.Append(rider);

                    string table1 = makeOneRecordTable(no, page * tableNum * rowNum, rowNum);
                    string table2 = makeOneRecordTable(no, page * tableNum * rowNum + rowNum, rowNum);
                    string table3 = makeOneRecordTable(no, page * tableNum * rowNum + rowNum * 2, rowNum);

                    string div =
                            "<table width=\"100%\"><tr>" +
                            "<td width=\"32%\">" + table1 + "</td>" +
                            "<td width=\"2%\"></td>" +
                            "<td width=\"32%\">" + table2 + "</td>" +
                            "<td width=\"2%\"></td>" +
                            "<td width=\"32%\">" + table3 + "</td>" +
                            "</tr></table>\r\n";

                    sb.Append(div);
                    sb.Append("<div class=\"pagebreak\"></div>\r\n");
                }
            }
            else
            {
                sb.Append(date);
                sb.Append(title);
                sb.Append(rider);
                sb.Append("<div class=\"pagebreak\"></div>\r\n");
            }

            string style = ".pagebreak { break-after: page; }";
            string header = "<header>\r\n<title>" + this.RaceNameTextBox.Text + " 個人成績</title>\r\n<style>\r\n" + style + "</style>\r\n</header>\r\n";
            string html = header + "<body>\r\n" + sb.ToString() + "\r\n</body>";

            File.WriteAllText(path, html);
        }

        private string makeOneRecordTable(string no, int startIndex, int rowNum)
        {
            List<string> record = this.records[no];

            if (startIndex > record.Count)
            {
                return "";
            }

            int endIndex = startIndex + rowNum - 1;
            if (endIndex > record.Count - 1)
            {
                endIndex = record.Count - 1;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
            {
                int lap = i + 1;
                sb.Append(
                    "<tr><td align=\"center\">" + lap.ToString() + "</a></td><td>" + record[i] + "</td></tr>\r\n"
                );
                lap++;

            }

            string table =
                "<table border=\"1\" width=\"100%\">\r\n" +
                "<tr><th>Lap数</th><th>" +
                "最終Laptime" +
                "</th></tr>\r\n" +
                sb.ToString() +
                "</table>\r\n";
            return table;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.clearCache();
        }


        static string myReplacer(Match m)
        {
            switch (m.Value)
            {
                case "０": return "0";
                case "１": return "1";
                case "２": return "2";
                case "３": return "3";
                case "４": return "4";
                case "５": return "5";
                case "６": return "6";
                case "７": return "7";
                case "８": return "8";
                case "９": return "9";
                default: return m.Value; 
            }
        }
    }
}
