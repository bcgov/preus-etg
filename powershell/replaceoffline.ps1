param([string]$template,[string]$file,[hashtable[]]$values,[int]$duration)

function GetDateTime([string]$pattern, [int]$minute = 0)
{
    $datetime = (Get-Date).AddMinutes($minute)
    $format = $pattern.Substring(13)
    return $datetime.ToString($format)
}

$text = Get-Content $template -Raw

Foreach ($value in $values)
{
    if ($value.val.StartsWith("#DateTimeNow:"))
    {
        $value.val = GetDateTime -pattern $value.val
    }
    elseif($value.val.StartsWith("#DateTimeAdd:"))
    {
        $value.val = GetDateTime -pattern $value.val -minute $duration
    }
    $text = $text.replace('#{' + $value.tag + '}', $value.val)
}

$text = $text.replace('#{maintenance-duration}', '{0} Minute(s)' -f $duration)

Set-Content $file $text
