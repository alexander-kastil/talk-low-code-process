$env = "dev"
$grp = "agentic-process-$env"
$loc = "westeurope"
az webapp up -n purchasing-mcp-$env -g $grp -p process-purchasing-plan-$env -l $loc -r "dotnet:9"