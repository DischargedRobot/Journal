import { ReactNode } from "react"

export const Label = ({
	className,
	children,
}: {
	className?: string
	children: ReactNode
}) => {
	return <span className={className}>{children}</span>
}
