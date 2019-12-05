function FindProxyForURL(url, host) {
  if (dnsDomainIs(host, "gf-game.girlfrontline.co.kr")) {
    return {0};
  } else if (shExpMatch(host, "*.ppgame.com")) {
	return {0};
  } else if (shExpMatch(host, "*.sunborngame.com")) {
	return {0};
  } else if (shExpMatch(host, "*.txwy.tw")) {
	return {0};
  } else {
    return "DIRECT";
  } 
}