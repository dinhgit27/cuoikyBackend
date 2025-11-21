// Simple auth helper for storing token and user info
const AUTH_TOKEN_KEY = 'api_token_v1';
const AUTH_USER_KEY = 'api_user_v1';

function setAuth(token, user){
  localStorage.setItem(AUTH_TOKEN_KEY, token);
  localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user || {}));
}

function getToken(){
  return localStorage.getItem(AUTH_TOKEN_KEY);
}

function getUser(){
  try{ return JSON.parse(localStorage.getItem(AUTH_USER_KEY) || '{}'); }catch{ return {}; }
}

function getRole(){
  const u = getUser(); return u?.role || null;
}

function logout(){
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_USER_KEY);
}

async function fetchAuth(url, opts = {}){
  const token = getToken();
  opts.headers = opts.headers || {};
  if (token) opts.headers['Authorization'] = 'Bearer ' + token;
  return await fetch(url, opts);
}

// expose to global
window.setAuth = setAuth;
window.getToken = getToken;
window.getUser = getUser;
window.getRole = getRole;
window.logout = logout;
window.fetchAuth = fetchAuth;
