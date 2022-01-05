using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
            regist(no);
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
            if (e.KeyChar ==(char)Keys.Enter)
            {
                regist(DriverNumberTextBox.Text);
                DriverNumberTextBox.Text = "";
            }
        }

        // 結果表示
        private void resultViewButton_Click(object sender, EventArgs e)
        {
            string tmpFile =  makeResultHtml();

            string url = "file://" + tmpFile.Replace("\\", "/");
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

        // HTMLファイルを生成してtmpファイルを作る
        private string makeResultHtml()
        {
            string tmpPath = Path.GetTempPath() + "result.html";            
            int rank = 1;

            StringBuilder sb = new StringBuilder();
            foreach (DataRow r in this.driverTable.Rows)
            {
                string recordPath = Path.GetTempPath() + r[0] + ".html";
                sb.Append(
                    "<tr><td align=\"center\">" + rank.ToString() +
                    "</td><td align=\"center\">" + r[0].ToString() +
                    "</td><td align=\"center\"><a href=\"" + recordPath + "\">" + r[1].ToString() +
                    "</a></td><td align=\"center\">" + r[2].ToString() +
                    "</td><td align=\"center\">" + r[3].ToString() + "</td></tr>\r\n"
                );

                makeRecordhtml(r[0].ToString(), r[1].ToString(), recordPath);

                rank++;
            }

            string table = 
                "<table border=\"1\">\r\n" + 
                "<tr><th>順位</th><th>" + 
                RaceRecorder.NO +
                "</th><th>" + 
                RaceRecorder.NAME + 
                "</th><th>" + 
                RaceRecorder.LAP + 
                "</th><th>" + 
                RaceRecorder.LAPTIME + 
                "</th></tr>\r\n" + 
                sb.ToString() + 
                "</table>\r\n";

            string date = "<h3>" + DateTime.Now.ToLocalTime().ToShortDateString() + "</h3>\r\n";
            string title = "<h1>" + this.RaceNameTextBox.Text + "</h1>\r\n";

            string header = "<header><title>" + this.RaceNameTextBox.Text +"</title></header>";
            string html = header + "<body>" + date + title + table + "</body>";
            File.WriteAllText(tmpPath, html);

            return tmpPath;
        }

        private void makeRecordhtml(string no, string name, string path)
        {
            int lap = 1;
            List<string> record = this.records[no];
            StringBuilder sb = new StringBuilder();
            foreach (string time in record)
            {
                sb.Append(
                  "<tr><td align=\"center\">" + lap.ToString() + "</a></td><td>" + time + "</td></tr>\r\n"
                );
                lap++;
            }

            string table =
             "<table border=\"1\">\r\n" +
             "<th>" +
             RaceRecorder.LAP +
             "</th><th>" +
             RaceRecorder.LAPTIME +
             "</th></tr>\r\n" +
             sb.ToString() +
             "</table>\r\n";

            string date = "<h3>" + DateTime.Now.ToLocalTime().ToShortDateString() + "</h3>\r\n";
            string title = "<h1>" + this.RaceNameTextBox.Text + "</h1>\r\n";
            string rider = "<h2>" + name +"</h2>\r\n";

            string header = "<header><title>" + this.RaceNameTextBox.Text + "</title></header>";
            string html = header + "<body>" + date + title + rider + table + "</body>";
            File.WriteAllText(path, html);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.clearCache();
        }
    }
}
