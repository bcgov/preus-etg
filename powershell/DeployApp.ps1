$sourcePath = $args[0]
$destPath - $args[1]
Copy-Item $sourcePath $destPath -Recurse -Force
