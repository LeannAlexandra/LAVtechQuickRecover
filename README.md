# LAVtechQuickRecover

A small UI for recovery tool to extract specific file types from a drive. 

Extract all pictures into one folder quickly and easily. 

//FOUNDATIONAL IDEA: make this batch code run with UI
`cd "F:\"
mkdir allpictures
for /R "F:\ADV REC" %%G in (*.png *.jpg *.jpeg *.gif) do copy "%%G" "F:\allpictures\"
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
