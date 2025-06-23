const sidebarToggle = document.getElementById("sidebarToggle");
const sidebar = document.getElementById("sidebar");
const sidebarOverlay = document.getElementById("sidebarOverlay");
const mainContent = document.querySelector(".main-content");
const navLinks = sidebar.querySelectorAll(".nav-link");

function toggleSidebar() {
  sidebar.classList.toggle("show");
  sidebarOverlay.classList.toggle("show");

  // Prevent body scroll when sidebar is open on mobile
  if (sidebar.classList.contains("show")) {
    document.body.style.overflow = "hidden";
  } else {
    document.body.style.overflow = "";
  }
}

function closeSidebar() {
  sidebar.classList.remove("show");
  sidebarOverlay.classList.remove("show");
  document.body.style.overflow = "";
}

// Event listeners
if (sidebarToggle) {
  sidebarToggle.addEventListener("click", toggleSidebar);
}
if (sidebarOverlay) {
  sidebarOverlay.addEventListener("click", closeSidebar);
}

// Close sidebar when clicking on nav links on mobile
navLinks.forEach(function (link) {
  link.addEventListener("click", function () {
    if (window.innerWidth <= 768) {
      closeSidebar();
    }
  });
});

// Handle window resize
window.addEventListener("resize", function () {
  if (window.innerWidth > 768) {
    closeSidebar();
  }
});
