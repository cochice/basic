깃과 깃허브 첫 실습

cd "프로젝트경로"
Get-ChildItem -Path . -Filter *.java -Recurse | ForEach-Object {
    $content = [System.IO.File]::ReadAllText($_.FullName, [System.Text.Encoding]::GetEncoding("euc-kr"))
    [System.IO.File]::WriteAllText($_.FullName, $content, [System.Text.Encoding]::UTF8)
}
