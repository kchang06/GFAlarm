function FindProxyForURL(url, host) {
  /* GFAlarm Proxy */
  if (dnsDomainIs(host, "gf-game.girlfrontline.co.kr") || // Korea
      shExpMatch(host, "*.ppgame.com") || // China
      shExpMatch(host, "*.sunborngame.com") || // Global/Japan
      shExpMatch(host, "*.txwy.tw")) { // Taiwan
    return {0};
  } else {
    return "DIRECT";
  } 
}