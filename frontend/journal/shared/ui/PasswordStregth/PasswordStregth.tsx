import { memo, useCallback, useMemo } from "react"

interface Props {
	password: string
}
const messages = ["Cлабый", "Средний", "Сильный", "Очень сильный"]
const calculateStrength = (password: string) => {
	let strength = 0
	if (password.length >= 8) {
		strength += 1
	}
	if (/\d/.test(password)) {
		strength += 1
	}
	if (/\W/.test(password)) {
		strength += 1
	}

	return Math.min(strength, messages.length - 1)
}

const selectColorStrength = (strength: number) => {
	switch (strength) {
		case 0:
			return "bg-gray-300"
		case 1:
			return "bg-red-500"
		case 2:
			return "bg-yellow-500"
		case 3:
			return "bg-green-500"
		default:
			return "bg-gray-300"
	}
}

const PasswordStregth = ({ password }: Props) => {
	const strength = useMemo(() => calculateStrength(password), [password])
	const colorStrength = useMemo(
		() => selectColorStrength(strength),
		[strength],
	)

	return (
		<div className="flex justify-between gap-6 w-full ">
			<div
				className={`w-full h-2 rounded-full bg-gray-300 ${colorStrength}`}
			></div>
			<div
				className={`w-full h-2 rounded-full bg-gray-300 ${strength > 1 ? colorStrength : ""}`}
			></div>
			<div
				className={`w-full h-2 rounded-full bg-gray-300 ${strength > 2 ? colorStrength : ""}`}
			></div>
			<span>{messages[strength]}</span>
		</div>
	)
}

export default memo(PasswordStregth)
