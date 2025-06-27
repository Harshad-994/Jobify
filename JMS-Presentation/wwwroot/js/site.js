document.querySelectorAll(".authPage .form-control").forEach((input) => {
  input.addEventListener("focus", function () {
    this.parentElement.style.transform = "scale(1.02)";
  });

  input.addEventListener("blur", function () {
    this.parentElement.style.transform = "scale(1)";
  });
});

function debounce(func, wait = 200, immediate) {
  var timeout;
  return function () {
    var context = this,
      args = arguments;
    var later = function () {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };
    var callNow = immediate && !timeout;
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
    if (callNow) func.apply(context, args);
  };
}

function formatDateTimeToDate(isoString) {
  if (!isoString) return "";
  const date = new Date(isoString);

  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const year = date.getFullYear();

  return `${day}-${month}-${year}`;
}

function formatDateOnly(dateStr) {
  // Expects dateStr in 'YYYY-MM-DD'
  if (!dateStr) return "";
  const [year, month, day] = dateStr.split("-");
  return `${day}-${month}-${year}`;
}

function timeAgoDateFormat(dateInput) {
  const now = new Date();
  const date = dateInput instanceof Date ? dateInput : new Date(dateInput);
  const diffMs = now - date;
  const diffSec = Math.floor(diffMs / 1000);
  const diffMin = Math.floor(diffSec / 60);
  const diffHr = Math.floor(diffMin / 60);
  const diffDay = Math.floor(diffHr / 24);

  if (diffDay < 1) {
    if (diffHr < 1) {
      if (diffMin < 1) return "just now";
      return `${diffMin} minute${diffMin === 1 ? "" : "s"} ago`;
    }
    return `${diffHr} hour${diffHr === 1 ? "" : "s"} ago`;
  }
  if (diffDay < 7) {
    return `${diffDay} day${diffDay === 1 ? "" : "s"} ago`;
  }
  if (diffDay < 28) {
    const weeks = Math.floor(diffDay / 7);
    return `${weeks} week${weeks === 1 ? "" : "s"} ago`;
  }
  const months = Math.floor(diffDay / 30);
  return `${months} month${months === 1 ? "" : "s"} ago`;
}

function isNullEmptyOrWhitespace(str) {
  return str === null || str.trim().length === 0;
}
