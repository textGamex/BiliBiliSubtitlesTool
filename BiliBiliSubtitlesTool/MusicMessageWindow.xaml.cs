using NLog;
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using NeteaseCloudMusicAPI.Api;
using NeteaseCloudMusicAPI.Api.Models;

namespace BiliBiliSubtitlesTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MusicMessageWindow : Window
    {
        private const int LIST_SIZE = 8;
        private readonly List<Lyrics> _lyrics = new List<Lyrics>(LIST_SIZE);
        private readonly List<Detail> _detail = new List<Detail>(LIST_SIZE);
        private long _tempSongId;
        private Lyrics _tempLyrics;
        private Detail _tempDetail;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MusicMessageWindow()
        {
            Log.Logger.SetLogger(Environment.GetCommandLineArgs());
            InitializeComponent();          
        }
        
        private void SearchMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongIdText.Text == string.Empty)
            {
                MessageBox.Show("请输入对应的信息!");
                return;
            }

            if (!TryGetSongId(SongIdText.Text, out long songId))
            {
                MessageBox.Show("格式错误!");
                return;
            }

            var lyricsInfo = CloudMusic.GetLyrics(songId);
            if (lyricsInfo.IsUncollected)
            {
                MessageBox.Show("歌词未收录或歌曲不存在");
                return;
            }
            _tempSongId = songId;
            _tempDetail = CloudMusic.GetDetails(songId);
            _tempLyrics = lyricsInfo;
            SongNameText.Text = _tempDetail.SongName;

            SingerNameText.Text = GetSingerName(_tempDetail.AuthorInfos);
            LyricsOutputBlock.Text = lyricsInfo.Lyric;
        }

        /// <summary>
        /// 尝试获得歌曲Id, 成功返回true,失败返回false
        /// </summary>
        /// <param name="text"></param>
        /// <param name="songIdString"></param>
        /// <returns>歌曲ID</returns>
        private bool TryGetSongId(string text, out long songId)
        {
            songId = 0;
            if (LinkTypeOption.SelectedItem == LinkType_Id)
            {
                if (!long.TryParse(text, out songId))
                {
                    _logger.Warn("ID格式错误, 原始数据={0}", SongIdText.Text);
                    return false;
                }
                return true;
            }

            if (!text.StartsWith("https://music.163.com"))
            {
                _logger.Debug("不以https开头, 链接={0}", text);
                return false;
            }
            string idStr;
            idStr = Regex.Match(text, @"id=(\d+)").Value;
            idStr = Regex.Replace(idStr, @"[^0-9]+", string.Empty);
            if (!long.TryParse(idStr, out songId))
            {
                return false;
            }
            return true;            
        }

        private string GetSingerName(System.Collections.Specialized.NameValueCollection authorInfos)
        {
            if (authorInfos == null)
            {
                var ex = new ArgumentNullException(nameof(authorInfos));
                _logger.Error(ex, "歌手信息集合为null, SongId={0}", _tempSongId);
                throw ex;
            }
            if (authorInfos.Count == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            var e = authorInfos.GetEnumerator();
            for (int i = 0, max = authorInfos.Count - 1; i < max; ++i)
            {
                e.MoveNext();
                sb.Append((string)e.Current).Append(" / ");
            }
            e.MoveNext();
            sb.Append((string)e.Current);
            return sb.ToString();
        }

        private void AddListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_tempLyrics == null || _tempDetail == null)
            {
                MessageBox.Show("没有数据!");
                return;
            }
            _lyrics.Add(_tempLyrics);
            _detail.Add(_tempDetail);
            SongsListText.Text = GetSongListText(_detail);
        }

        private string GetSongListText(IEnumerable<Detail> detailList)
        {
            var sb = new StringBuilder();
            uint count = 0;
            foreach (var item in detailList)
            {
                sb.Append($"{count++}. {item.SongName} - {GetSingerName(item.AuthorInfos)}").Append('\n');
            }
            return sb.ToString();
        }

        private void LinkTypeOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LinkTypeOption.SelectedItem == LinkType_Id)
            {
                int LongMaxLength = 19;
                SongIdText.PreviewTextInput += NumberOnly;
                SongIdText.MaxLength = LongMaxLength;
            }
            else
            {
                SongIdText.PreviewTextInput -= NumberOnly;
                SongIdText.MaxLength = 128;
            }
        }

        private void NumberOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
