# LAVtechQuickRecover

A small UI for recovery tool to extract specific file types from a drive. 
<a href="https://leannalexandra.github.io/LAVtechQuickRecover/" target="_blank">Accompanying Page</a>

Extract all pictures into one folder quickly and easily. 

//FOUNDATIONAL IDEA: make this batch code run with UI
`cd "F:\";
mkdir allpictures;
for /R "F:\ADV REC" %%G in (*.png *.jpg *.jpeg *.gif) do copy "%%G" "F:\allpictures\";
PAUSE`

//EXTRA FEATURES
Can pick source drive.
Can pick destination Drive
Can pick from a variety of different file types, pictures, videos, audio, documents. 
Can preserve folder hierarchy, or just extract into one folder 
Output folder can be customNamed 

//V1. Limitations: 
input must be a complete drive. - Does not selectively skip windows folders or appdata folder. 
folder selections are limited to predefined list
.Net runtime dependent 
output folder does not have a safeguard for empty foldername (yet);
no functionality for extended user setting yet
only limited programming extractions. (not git-efficient yet)
.txt files can get overwhelming real quick - specifically in document extraction.

<div class="intro">
        <h1>LAVTech QuickPick</h1>
        <p>Quickly herd your files, pictures or videos to a usb.</p>
    </div>
    <div class="features">
        <h2>Features</h2>
    <p>LAVTech Quick Pick is a small app that allows you to copy preselected files such as documents, video, audio or images from one drive into one folder.</p>
    <ul>
        <li>Copy all files of a given category Quickly
            <ul>
                <!--   private string[] fileOptions = { "Pictures", "Videos","Audio", "Documents", "Misc", "All" };
        string[] pictureExtensions = { ".png", ".jpg", ".jpeg", ".gif", "tiff", ".tif", ".bmp" };
        string[] vectorGraphicsExtentions = { ".EPS", ".AI", ".psd",".indd", ".raw", ".svg", ".cdr" };
        string[] audioExtensions = {".AIF", ".IFF", ".M3U", ".M4A", ".MID", ".MP3", ".MPA", ".WAV", ".WMA" };
        string[] videoExtensions = { ".3G2", ".3GP", ".ASF", ".AVI", ".FLV", ".M4V", ".MOV", ".MP4", ".MPG", ".RM", ".SRT", ".SWF", ".VOB", ".WMV" };
        string[] documentExtensions = { ".PDF", ".doc",".xlsx", "docx", ".xls",".html",".htm", ".ODT", ".ODS", ".PPT", ".PPTX", ".TXT" };
        string[] programmingExtentions = {".sln",".js", ".html", ".css", ".json", ".c", ".cpp", "cs", ".py", ".pyc", ".pyw", ".pyx", ".class" /*, "", "", "", "", "", "", ""*/ };  -->
            <li>Pictures: <span class="description-elaboration">includes - bmp, jpeg, jpg, png, gif, tif, tiff and some adobe and corel files (eps, ai, psd, indd, raw, svg and cdr)</span></li>
            <li>Video: <span class="description-elaboration">includes - 3g2, 3gp, asf, avi, flv, m4a, mov, mp4, mpg, rm, srt, swf, vob, wmv and mkv</span></li>
            <li>Audio: <span class="description-elaboration">includes - aif, iff, m3u, m4a, mid, mp3, mpa, wav, wma</span></li>
            <li>Documents: <span class="description-elaboration">includes - pdf, doc, xlsx, docx, xls, html, htm, odt, ods, ppt, pptx, txt</span></li>
            <li>Programming: <span class="description-elaboration"> Contains the most source code files * STILL IN DEVELOPMENT * - .sln, .js, .html, .css, .json, .c, .cpp, cs, .py, .pyc, .pyw, .pyx, .class </span></li>
            <li>Misc:  <span class="description-elaboration">- DISCLAIMER -  currently only supports .blend files</span></li>
            <li>All: <span class="description-elaboration">includes - all the above </span></li>
        </ul>
        </li>
        <li>Recursively goes through all folders and copies the file type to a custom folder on a drive.
            <ul><li>It skips the destination folder to prevent some nasty recursion ;)</li></ul>
        </li>
        <li>Currenlty does not overwrite same-named files.</li>
        <li>You can preserve the folder structure.</li>
        <li>Open to Suggestions for features such as these to look forward to:
            <ul>
                <li>TreeView: To select a specific input folder as opposed to the whole drive.</li>
                <li>Smarter: Auto-skip such as skipping all of the 'appdata' folder should you use it to retain a user's files and folders for a new computer with familiar files in all the familiar places.</li>
            </ul>
        </li>
    </ul>
    </div>
    <div class="help">
        <h2>How To:</h2>
        <p>Windows: To run the app, download the LAVTechQuickRecover.exe and run it.</p>
        <p>The application automatically reads and indexes the available permanent drives and removable drives (usb) defaults to E:/ (primarily flash drive/secondary storage). The program runs through all folders seeking the files with the predefined extentions and copies them to one folder. There is an option to preserve the folder structure, if you wish.
        </p>
    </div>
    <div class="limitations">
        <h2>Limitations</h2>
        <ol>
            <li>Does not copy duplicates based on name - does not rename duplicates: 
                <ul><li>Idea: compare ELF header, and copy & rename only when file is clear duplicate.</li></ul>
            </li>
            <li>does not automatically understand GIT and programming<span class="description-elaboration"> - I don't anticipte too much use - apart from true offline application thereoff</span></li>
            <li>Input is currently limited to a whole drive - small fix if needed</li>
        </ol>
    </div>
    <div class="special-thanks">
        <h2>Special Thanks</h2>
        <H3>Williams-Creativity</H3>
        <p>Special Thanks to Williams-Creativity for the image</p>
        <!-- <h3>Anieke - Writerrise</h3> // UNCOMMENT WHEN COPY IS RECEIVED ->s
        <p>Copy that sells - Thanks to Anieke for taking a back-hand code comments and scanty documentation into a presentable and understandable website</p>
    </div> -->
    <div class="suggestions">
        <h2>Any Suggestions?</h2>
        <p>If you have any suggestions or questions, please feel free to reach out via  <a href="mailto:leannalexandraviolet@gmail.com">email</a></p>
    </div>
    <div class="devlog">
    </div>
    <div class="about-author">
        <h2>About Me</h2>
        <p>Hi, I'm LeAnn Alexandra, but I go by Alexandra, because it is slightly less likely to be misspelled. I resigned my steady position as General Manager with a FMCG Company in early 2023 to pursue a carreer in software and programming. To make something work flawlessly is not enough for me, I am driven by my need to understand every minor voltage shift inside the hardware.</p>
    </div>
