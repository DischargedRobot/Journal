// билдер запроса
export function buildQuery(
	params: Record<string, string | number | boolean | unknown>,
) {
	const qs = Object.entries(params)
		.filter(([, v]) => v !== undefined && v !== null && v !== "")
		.map(
			([k, v]) =>
				`${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`,
		)
		.join("&")
	return qs.length ? `?${qs}` : ""
}
