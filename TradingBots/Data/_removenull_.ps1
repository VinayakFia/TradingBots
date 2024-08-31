$items = Get-ChildItem "*.csv" | Select-String -pattern "null" | Select-Object -ExpandProperty path | Get-Unique
ForEach ($item in $items){
    Remove-Item -Force $item
}