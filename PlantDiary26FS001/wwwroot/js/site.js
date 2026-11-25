// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {
  const loading = document.getElementById("navigation-loading");
  if (!loading) {
    return;
  }

  document.querySelectorAll("a:not([target='_blank'])").forEach((link) => {
    link.addEventListener("click", () => {
      loading.hidden = false;
    });
  });
});
