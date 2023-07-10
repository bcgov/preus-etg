param([string]$server,[string]$database,[string]$user,[string]$pass,[string]$backupDirectory,[string]$backupFile,[int]$daysToStoreBackups = 30)

SqlCmd -S $server -U $user -P $pass -Q "BACKUP DATABASE [$($database)] TO DISK='$($backupDirectory)\$($backupFile)'"

if ($daysToStoreBackups -gt 0) {
	Get-ChildItem "$backupDirectory\*.bak" |? { $_.lastwritetime -le (Get-Date).AddDays(-$daysToStoreBackups)} |% {Remove-Item $_ -force }  
	"removed all previous backups older than $daysToStoreBackups days"
}
