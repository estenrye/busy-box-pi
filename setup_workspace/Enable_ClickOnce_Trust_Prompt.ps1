$registry_path = 'HKLM:\SOFTWARE\Microsoft\.NETFramework\Security\TrustManager\PromptingLevel'
$prompt_levels = @{
	Internet = 'Enabled'
	UntrustedSites = 'Disabled'
	MyComputer = 'Enabled'
	LocalIntranet = 'Enabled'
	TrustedSites = 'Enabled'
}

foreach ($kv in $prompt_levels.GetEnumerator())
{
	Set-ItemProperty -Path $registry_path -Name $kv.Name -Value $kv.Value
}
